﻿using AssetRipper.Core;
using AssetRipper.Core.Classes;
using AssetRipper.Core.Classes.AudioClip;
using AssetRipper.Core.Interfaces;
using AssetRipper.Core.Logging;
using AssetRipper.Core.Parser.Files.SerializedFiles;
using AssetRipper.Core.Project;
using AssetRipper.Core.Project.Collections;
using AssetRipper.Core.Project.Exporters;
using System.IO;

namespace AssetRipper.Library.Exporters.Audio
{
	public class NativeAudioExporter : BinaryAssetExporter
	{
		public override bool IsHandle(IUnityObjectBase asset)
		{
			return asset is AudioClip;
		}

		public override IExportCollection CreateCollection(VirtualSerializedFile virtualFile, IUnityObjectBase asset)
		{
			return new AssetExportCollection(this, asset, GetExportExtension((AudioClip)asset));
		}

		public override bool Export(IExportContainer container, IUnityObjectBase asset, string path)
		{
			AudioClip audioClip = (AudioClip)asset;

			if (AudioClip.HasLoadType(container.Version))
			{
				if (audioClip.FSBResource.CheckIntegrity(audioClip.File))
				{
					byte[] data = audioClip.FSBResource.GetContent(audioClip.File);
					TaskManager.AddTask(File.WriteAllBytesAsync(path, data));
				}
				else
				{
					Logger.Log(LogType.Warning, LogCategory.Export, $"Can't export '{audioClip.GetValidName()}' because data can't be read from resources file '{audioClip.FSBResource.Source}'");
					return false;
				}
			}
			else
			{
				if (AudioClip.HasStreamingInfo(container.Version) && audioClip.LoadType == AudioClipLoadType.Streaming && audioClip.AudioData == null)
				{
					if (audioClip.StreamingInfo.CheckIntegrity(audioClip.File))
					{
						byte[] data = audioClip.StreamingInfo.GetContent(audioClip.File);
						TaskManager.AddTask(File.WriteAllBytesAsync(path, data));
					}
					else
					{
						Logger.Log(LogType.Warning, LogCategory.Export, $"Can't export '{audioClip.GetValidName()}' because resources file '{audioClip.StreamingInfo.Path}' hasn't been found");
						return false;
					}
				}
				else
				{
					TaskManager.AddTask(File.WriteAllBytesAsync(path, audioClip.AudioData));
				}
			}
			return true;
		}

		private static string GetExportExtension(AudioClip audioClip)
		{
			if (AudioClip.HasType(audioClip.File.Version))
			{
				switch (audioClip.Type)
				{
					case FMODSoundType.ACC:
						return "m4a";
					case FMODSoundType.AIFF:
						return "aif";
					case FMODSoundType.IT:
						return "it";
					case FMODSoundType.MOD:
						return "mod";
					case FMODSoundType.MPEG:
						return "mp3";
					case FMODSoundType.OGGVORBIS:
						return "ogg";
					case FMODSoundType.S3M:
						return "s3m";
					case FMODSoundType.WAV:
						return "wav";
					case FMODSoundType.XM:
						return "xm";
					case FMODSoundType.XMA:
						return "wav";
					case FMODSoundType.VAG:
						return "vag";
					case FMODSoundType.AUDIOQUEUE:
						return "fsb";
					default:
						return "audioClip";
				}
			}
			else
			{
				switch (audioClip.CompressionFormat)
				{
					case AudioCompressionFormat.PCM:
					case AudioCompressionFormat.ADPCM:
					case AudioCompressionFormat.Vorbis:
					case AudioCompressionFormat.MP3:
					case AudioCompressionFormat.GCADPCM:
						return "fsb";
					case AudioCompressionFormat.VAG:
					case AudioCompressionFormat.HEVAG:
						return "vag";
					case AudioCompressionFormat.XMA:
						return "wav";
					case AudioCompressionFormat.AAC:
						return "m4a";
					case AudioCompressionFormat.ATRAC9:
						return "at9";
					default:
						return "audioClip";
				}
			}
		}
	}
}
