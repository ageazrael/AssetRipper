﻿using AssetRipper.Core.IO.Endian;
using AssetRipper.Core.IO.Extensions;
using AssetRipper.Core.IO.FileReading;
using AssetRipper.Core.Parser.Files;
using AssetRipper.Core.Parser.Files.SerializedFiles.Parser.TypeTree;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AssetAnalyzer
{
	public class SerializedFile
	{
		public FileReader reader;
		public string fullName;
		public string originalPath;
		public string fileName;
		public int[] version = { 0, 0, 0, 0 };
		public BuildType buildType;

		public SerializedFileHeader header;
		private byte m_FileEndianess;
		public string unityVersion = "2.5.0f5";
		public Platform m_TargetPlatform = Platform.UnknownPlatform;
		private bool m_EnableTypeTree = true;
		public List<SerializedType> m_Types;
		public int bigIDEnabled = 0;
		public List<ObjectInfo> m_Objects;
		private List<LocalSerializedObjectIdentifier> m_ScriptTypes;
		public List<FileIdentifier> m_Externals;
		public List<SerializedType> m_RefTypes;
		public string userInformation;

		public SerializedFile(FileReader reader)
		{
			this.reader = reader;
			fullName = reader.FullPath;
			fileName = reader.FileName;

			// ReadHeader
			header = new SerializedFileHeader();
			header.m_MetadataSize = reader.ReadUInt32();
			header.m_FileSize = reader.ReadUInt32();
			header.m_Version = (SerializedFileFormatVersion)reader.ReadUInt32();
			header.m_DataOffset = reader.ReadUInt32();

			if (header.m_Version >= SerializedFileFormatVersion.kUnknown_9)
			{
				header.m_Endianess = reader.ReadByte();
				header.m_Reserved = reader.ReadBytes(3);
				m_FileEndianess = header.m_Endianess;
			}
			else
			{
				reader.BaseStream.Position = header.m_FileSize - header.m_MetadataSize;
				m_FileEndianess = reader.ReadByte();
			}

			if (header.m_Version >= SerializedFileFormatVersion.kLargeFilesSupport)
			{
				header.m_MetadataSize = reader.ReadUInt32();
				header.m_FileSize = reader.ReadInt64();
				header.m_DataOffset = reader.ReadInt64();
				reader.ReadInt64(); // unknown
			}

			// ReadMetadata
			if (m_FileEndianess == 0)
			{
				reader.EndianType = EndianType.LittleEndian;
			}
			if (header.m_Version >= SerializedFileFormatVersion.kUnknown_7)
			{
				unityVersion = reader.ReadStringToNull();
				SetVersion(unityVersion);
			}
			if (header.m_Version >= SerializedFileFormatVersion.kUnknown_8)
			{
				m_TargetPlatform = (Platform)reader.ReadInt32();
				if (!Enum.IsDefined(typeof(Platform), m_TargetPlatform))
				{
					m_TargetPlatform = Platform.UnknownPlatform;
				}
			}
			if (header.m_Version >= SerializedFileFormatVersion.kHasTypeTreeHashes)
			{
				m_EnableTypeTree = reader.ReadBoolean();
			}

			// Read Types
			int typeCount = reader.ReadInt32();
			m_Types = new List<SerializedType>(typeCount);
			for (int i = 0; i < typeCount; i++)
			{
				m_Types.Add(ReadSerializedType(false));
			}

			if (header.m_Version >= SerializedFileFormatVersion.kUnknown_7 && header.m_Version < SerializedFileFormatVersion.kUnknown_14)
			{
				bigIDEnabled = reader.ReadInt32();
			}

			// Read Objects
			int objectCount = reader.ReadInt32();
			m_Objects = new List<ObjectInfo>(objectCount);
			for (int i = 0; i < objectCount; i++)
			{
				var objectInfo = new ObjectInfo();
				if (bigIDEnabled != 0)
				{
					objectInfo.m_PathID = reader.ReadInt64();
				}
				else if (header.m_Version < SerializedFileFormatVersion.kUnknown_14)
				{
					objectInfo.m_PathID = reader.ReadInt32();
				}
				else
				{
					reader.AlignStream();
					objectInfo.m_PathID = reader.ReadInt64();
				}

				if (header.m_Version >= SerializedFileFormatVersion.kLargeFilesSupport)
					objectInfo.byteStart = reader.ReadInt64();
				else
					objectInfo.byteStart = reader.ReadUInt32();

				objectInfo.byteStart += header.m_DataOffset;
				objectInfo.byteSize = reader.ReadUInt32();
				objectInfo.typeID = reader.ReadInt32();
				if (header.m_Version < SerializedFileFormatVersion.kRefactoredClassId)
				{
					objectInfo.classID = reader.ReadUInt16();
					objectInfo.serializedType = m_Types.Find(x => x.classID == objectInfo.typeID);
				}
				else
				{
					var type = m_Types[objectInfo.typeID];
					objectInfo.serializedType = type;
					objectInfo.classID = type.classID;
				}
				if (header.m_Version < SerializedFileFormatVersion.kHasScriptTypeIndex)
				{
					objectInfo.isDestroyed = reader.ReadUInt16();
				}
				if (header.m_Version >= SerializedFileFormatVersion.kHasScriptTypeIndex && header.m_Version < SerializedFileFormatVersion.kRefactorTypeData)
				{
					var m_ScriptTypeIndex = reader.ReadInt16();
					if (objectInfo.serializedType != null)
						objectInfo.serializedType.m_ScriptTypeIndex = m_ScriptTypeIndex;
				}
				if (header.m_Version == SerializedFileFormatVersion.kSupportsStrippedObject || header.m_Version == SerializedFileFormatVersion.kRefactoredClassId)
				{
					objectInfo.stripped = reader.ReadByte();
				}
				m_Objects.Add(objectInfo);
			}

			if (header.m_Version >= SerializedFileFormatVersion.kHasScriptTypeIndex)
			{
				int scriptCount = reader.ReadInt32();
				m_ScriptTypes = new List<LocalSerializedObjectIdentifier>(scriptCount);
				for (int i = 0; i < scriptCount; i++)
				{
					var m_ScriptType = new LocalSerializedObjectIdentifier();
					m_ScriptType.localSerializedFileIndex = reader.ReadInt32();
					if (header.m_Version < SerializedFileFormatVersion.kUnknown_14)
					{
						m_ScriptType.localIdentifierInFile = reader.ReadInt32();
					}
					else
					{
						reader.AlignStream();
						m_ScriptType.localIdentifierInFile = reader.ReadInt64();
					}
					m_ScriptTypes.Add(m_ScriptType);
				}
			}

			int externalsCount = reader.ReadInt32();
			m_Externals = new List<FileIdentifier>(externalsCount);
			for (int i = 0; i < externalsCount; i++)
			{
				var m_External = new FileIdentifier();
				if (header.m_Version >= SerializedFileFormatVersion.kUnknown_6)
				{
					var tempEmpty = reader.ReadStringToNull();
				}
				if (header.m_Version >= SerializedFileFormatVersion.kUnknown_5)
				{
					m_External.guid = new Guid(reader.ReadBytes(16));
					m_External.type = reader.ReadInt32();
				}
				m_External.pathName = reader.ReadStringToNull();
				m_External.fileName = Path.GetFileName(m_External.pathName);
				m_Externals.Add(m_External);
			}

			if (header.m_Version >= SerializedFileFormatVersion.kSupportsRefObject)
			{
				int refTypesCount = reader.ReadInt32();
				m_RefTypes = new List<SerializedType>(refTypesCount);
				for (int i = 0; i < refTypesCount; i++)
				{
					m_RefTypes.Add(ReadSerializedType(true));
				}
			}

			if (header.m_Version >= SerializedFileFormatVersion.kUnknown_5)
			{
				userInformation = reader.ReadStringToNull();
			}

			//reader.AlignStream(16);
		}

		public void SetVersion(string stringVersion)
		{
			if (stringVersion != strippedVersion)
			{
				unityVersion = stringVersion;
				var buildSplit = Regex.Replace(stringVersion, @"\d", "").Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries);
				buildType = new BuildType(buildSplit[0]);
				var versionSplit = Regex.Replace(stringVersion, @"\D", ".").Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries);
				version = versionSplit.Select(int.Parse).ToArray();
			}
		}

		private SerializedType ReadSerializedType(bool isRefType)
		{
			var type = new SerializedType();

			type.classID = reader.ReadInt32();

			if (header.m_Version >= SerializedFileFormatVersion.kRefactoredClassId)
			{
				type.m_IsStrippedType = reader.ReadBoolean();
			}

			if (header.m_Version >= SerializedFileFormatVersion.kRefactorTypeData)
			{
				type.m_ScriptTypeIndex = reader.ReadInt16();
			}

			if (header.m_Version >= SerializedFileFormatVersion.kHasTypeTreeHashes)
			{
				if (isRefType && type.m_ScriptTypeIndex >= 0)
				{
					type.m_ScriptID = reader.ReadBytes(16);
				}
				else if (header.m_Version < SerializedFileFormatVersion.kRefactoredClassId && type.classID < 0 || header.m_Version >= SerializedFileFormatVersion.kRefactoredClassId && type.classID == 114)
				{
					type.m_ScriptID = reader.ReadBytes(16);
				}
				type.m_OldTypeHash = reader.ReadBytes(16);
			}

			if (m_EnableTypeTree)
			{
				type.m_Type = new TypeTree();
				type.m_Type.Nodes = new List<TypeTreeNode>();
				if (header.m_Version >= SerializedFileFormatVersion.kUnknown_12 || header.m_Version == SerializedFileFormatVersion.kUnknown_10)
				{
					TypeTreeBlobRead(type.m_Type);
				}
				else
				{
					ReadTypeTree(type.m_Type);
				}
				if (header.m_Version >= SerializedFileFormatVersion.kStoresTypeDependencies)
				{
					if (isRefType)
					{
						type.m_KlassName = reader.ReadStringToNull();
						type.m_NameSpace = reader.ReadStringToNull();
						type.m_AsmName = reader.ReadStringToNull();
					}
					else
					{
						type.m_TypeDependencies = reader.ReadInt32Array();
					}
				}
			}

			return type;
		}

		private void ReadTypeTree(TypeTree m_Type, int level = 0)
		{
			var typeTreeNode = new TypeTreeNode();
			m_Type.Nodes.Add(typeTreeNode);
			typeTreeNode.Level = (byte)level;
			typeTreeNode.Type = reader.ReadStringToNull();
			typeTreeNode.Name = reader.ReadStringToNull();
			typeTreeNode.ByteSize = reader.ReadInt32();
			if (header.m_Version == SerializedFileFormatVersion.kUnknown_2)
			{
				var variableCount = reader.ReadInt32();
			}
			if (header.m_Version != SerializedFileFormatVersion.kUnknown_3)
			{
				typeTreeNode.Index = reader.ReadInt32();
			}
			typeTreeNode.TypeFlags = reader.ReadInt32();
			typeTreeNode.Version = reader.ReadInt32();
			if (header.m_Version != SerializedFileFormatVersion.kUnknown_3)
			{
				typeTreeNode.MetaFlag = (AssetRipper.Core.Parser.Files.SerializedFiles.Parser.TransferMetaFlags)reader.ReadInt32();
			}

			int childrenCount = reader.ReadInt32();
			for (int i = 0; i < childrenCount; i++)
			{
				ReadTypeTree(m_Type, level + 1);
			}
		}

		private void TypeTreeBlobRead(TypeTree m_Type)
		{
			int numberOfNodes = reader.ReadInt32();
			int stringBufferSize = reader.ReadInt32();
			for (int i = 0; i < numberOfNodes; i++)
			{
				var typeTreeNode = new TypeTreeNode();
				m_Type.Nodes.Add(typeTreeNode);
				typeTreeNode.Version = reader.ReadUInt16();
				typeTreeNode.Level = reader.ReadByte();
				typeTreeNode.TypeFlags = reader.ReadByte();
				typeTreeNode.TypeStrOffset = reader.ReadUInt32();
				typeTreeNode.NameStrOffset = reader.ReadUInt32();
				typeTreeNode.ByteSize = reader.ReadInt32();
				typeTreeNode.Index = reader.ReadInt32();
				typeTreeNode.MetaFlag = (AssetRipper.Core.Parser.Files.SerializedFiles.Parser.TransferMetaFlags)reader.ReadInt32();
				if (header.m_Version >= SerializedFileFormatVersion.kTypeTreeNodeWithTypeFlags)
				{
					typeTreeNode.RefTypeHash = reader.ReadUInt64();
				}
			}
			m_Type.StringBuffer = reader.ReadBytes(stringBufferSize);

			using (var stringBufferReader = new BinaryReader(new MemoryStream(m_Type.StringBuffer)))
			{
				for (int i = 0; i < numberOfNodes; i++)
				{
					var m_Node = m_Type.Nodes[i];
					m_Node.Type = ReadString(stringBufferReader, m_Node.TypeStrOffset);
					m_Node.Name = ReadString(stringBufferReader, m_Node.NameStrOffset);
				}
			}

			string ReadString(BinaryReader stringBufferReader, uint value)
			{
				var isOffset = (value & 0x80000000) == 0;
				if (isOffset)
				{
					stringBufferReader.BaseStream.Position = value;
					return stringBufferReader.ReadStringToNull();
				}
				var offset = value & 0x7FFFFFFF;
				if (CommonString.StringBuffer.TryGetValue(offset, out var str))
				{
					return str;
				}
				return offset.ToString();
			}
		}

		public bool IsVersionStripped => unityVersion == strippedVersion;

		private const string strippedVersion = "0.0.0";
	}
}
