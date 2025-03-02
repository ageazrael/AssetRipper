﻿using AssetRipper.Core.IO.Asset;
using AssetRipper.Core.IO.Extensions;
using AssetRipper.Core.Project;
using AssetRipper.Core.YAML;
using AssetRipper.Core.YAML.Extensions;
using System;
using UnityVersion = AssetRipper.Core.Parser.Files.UnityVersion;

namespace AssetRipper.Core.Classes.Misc
{
	public struct MdFour : IAsset
	{
		public static int ToSerializedVersion(UnityVersion version)
		{
			// unknown version
			return 2;
		}

		public void Read(AssetReader reader)
		{
			Md4Hash = reader.ReadByteArray();
			reader.AlignStream();
		}

		public void Write(AssetWriter writer)
		{
			Md4Hash.Write(writer);
			writer.AlignStream();
		}

		public YAMLNode ExportYAML(IExportContainer container)
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.AddSerializedVersion(ToSerializedVersion(container.Version));
			node.Add(Md4HashName, Md4Hash.ExportYAML());
			return node;
		}

		public override string ToString()
		{
			if (Md4Hash == null)
			{
				return base.ToString();
			}
			uint data0 = BitConverter.ToUInt32(Md4Hash, 0);
			uint data1 = BitConverter.ToUInt32(Md4Hash, 4);
			uint data2 = BitConverter.ToUInt32(Md4Hash, 8);
			uint data3 = BitConverter.ToUInt32(Md4Hash, 12);
			return $"{data3:x8}{data2:x8}{data1:x8}{data0:x8}";
		}

		public byte[] Md4Hash { get; set; }

		public const string Md4HashName = "md4 hash";
	}
}
