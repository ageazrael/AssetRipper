using AssetRipper.Core.IO.Asset;
using AssetRipper.Core.IO.Extensions;
using AssetRipper.Core.Layout;
using AssetRipper.Core.Math;
using AssetRipper.Core.Parser.Asset;
using AssetRipper.Core.Project;
using AssetRipper.Core.YAML;

namespace AssetRipper.Core.Classes.AudioMixer
{
	public sealed class AudioHighPassFilter : Behaviour
	{
		public AudioHighPassFilter(LayoutInfo layout) : base(layout){}
		public AudioHighPassFilter(AssetInfo assetInfo) : base(assetInfo) { }

		public override void Read(AssetReader reader)
		{
			base.Read(reader);

			CutoffFrequency = reader.ReadSingle();
			HighpassResonanceQ = reader.ReadSingle();
		}

		public override void Write(AssetWriter writer)
		{
			base.Write(writer);
			writer.Write(CutoffFrequency);
			writer.Write(HighpassResonanceQ);
		}

		protected override YAMLMappingNode ExportYAMLRoot(IExportContainer container)
		{
			YAMLMappingNode node = base.ExportYAMLRoot(container);
			node.Add(CutoffFrequencyName, CutoffFrequency);
			node.Add(HighpassResonanceQName, HighpassResonanceQ);
			return node;
		}

		public float CutoffFrequency { get; set; }
		public float HighpassResonanceQ { get; set; }

		public const string CutoffFrequencyName = "m_CutoffFrequency";
		public const string HighpassResonanceQName = "m_HighpassResonanceQ";
	}
}
