using AssetRipper.Core.IO.Asset;
using AssetRipper.Core.IO.Extensions;
using AssetRipper.Core.Layout;
using AssetRipper.Core.Math;
using AssetRipper.Core.Parser.Asset;
using AssetRipper.Core.Project;
using AssetRipper.Core.YAML;

namespace AssetRipper.Core.Classes.AudioMixer
{
	public sealed class AudioReverbFilter : Behaviour
	{
		public AudioReverbFilter(LayoutInfo layout) : base(layout) { }
		public AudioReverbFilter(AssetInfo assetInfo) : base(assetInfo) { }

		public override void Read(AssetReader reader)
		{
			base.Read(reader);

			DryLevel = reader.ReadSingle();
			Room = reader.ReadSingle();
			RoomHF = reader.ReadSingle();
			DecayTime = reader.ReadSingle();
			DecayHFRatio = reader.ReadSingle();
			ReflectionsLevel = reader.ReadSingle();
			ReverbLevel = reader.ReadSingle();
			ReverbDelay = reader.ReadSingle();
			Diffusion = reader.ReadSingle();

			Density = reader.ReadSingle();
			HFReference = reader.ReadSingle();
			RoomLF = reader.ReadSingle();
			LFReference = reader.ReadSingle();
			ReflectionsDelay = reader.ReadSingle();

			ReverbPreset = reader.ReadInt32();
		}

		public override void Write(AssetWriter writer)
		{
			base.Write(writer);
			writer.Write(DryLevel);
			writer.Write(Room);
			writer.Write(RoomHF);
			writer.Write(DecayTime);
			writer.Write(DecayHFRatio);
			writer.Write(ReflectionsLevel);
			writer.Write(ReverbLevel);
			writer.Write(ReverbDelay);
			writer.Write(Diffusion);

			writer.Write(Density);
			writer.Write(HFReference);
			writer.Write(RoomLF);
			writer.Write(LFReference);
			writer.Write(ReflectionsDelay);
			writer.Write(ReverbPreset);
		}

		protected override YAMLMappingNode ExportYAMLRoot(IExportContainer container)
		{
			YAMLMappingNode node = base.ExportYAMLRoot(container);
			node.Add(DryLevelName, DryLevel);
			node.Add(RoomName, Room);
			node.Add(RoomHFName, RoomHF);
			node.Add(DecayTimeName, DecayTime);
			node.Add(DecayHFRatioName, DecayHFRatio);
			node.Add(ReflectionsLevelName, ReflectionsLevel);
			node.Add(ReverbLevelName, ReverbLevel);
			node.Add(ReverbDelayName, ReverbDelay);
			node.Add(DiffusionName, Diffusion);

			node.Add(DensityName, Density);
			node.Add(HFReferenceName, HFReference);
			node.Add(RoomLFName, RoomLF);
			node.Add(LFReferenceName, LFReference);
			node.Add(ReflectionsDelayName, ReflectionsDelay);
			node.Add(ReverbPresetName, ReverbPreset);
			return node;
		}

		public float DryLevel { get; set; }
		public float Room { get; set; }
		public float RoomHF { get; set; }
		public float DecayTime { get; set; }
		public float DecayHFRatio { get; set; }
		public float ReflectionsLevel { get; set; }
		public float ReverbLevel { get; set; }
		public float ReverbDelay { get; set; }
		public float Diffusion { get; set; }

		public float Density { get; set; }
		public float HFReference { get; set; }
		public float RoomLF { get; set; }
		public float LFReference { get; set; }
		public float ReflectionsDelay { get; set; }
		public int ReverbPreset { get; set; }

		public const string DryLevelName = "m_DryLevel";
		public const string RoomName = "m_Room";
		public const string RoomHFName = "m_RoomHF";
		public const string DecayTimeName = "m_DecayTime";
		public const string DecayHFRatioName = "m_DecayHFRatio";
		public const string ReflectionsLevelName = "m_ReflectionsLevel";
		public const string ReverbLevelName = "m_ReverbLevel";
		public const string ReverbDelayName = "m_ReverbDelay";
		public const string DiffusionName = "m_Diffusion";

		public const string DensityName = "m_Density";
		public const string HFReferenceName = "m_HFReference";
		public const string RoomLFName = "m_RoomLF";
		public const string LFReferenceName = "m_LFReference";
		public const string ReflectionsDelayName = "m_ReflectionsDelay";
		public const string ReverbPresetName = "m_ReverbPreset";
	}
}
