using AssetRipper.Core.IO.Asset;
using AssetRipper.Core.IO.Extensions;
using AssetRipper.Core.Layout;
using AssetRipper.Core.Math;
using AssetRipper.Core.Parser.Asset;
using AssetRipper.Core.Project;
using AssetRipper.Core.YAML;

namespace AssetRipper.Core.Classes.AudioMixer
{
	public struct Keyframe : IAssetReadable, IAssetWritable, IYAMLExportable
	{
		public Keyframe()
		{
			this.Time = 0;
			this.Value = 0;
			this.InSlope = 0;
			this.OutSlope = 0;

			this.TangentMode = 0;
			this.WeightedMode = 0;
			this.InWeight = 0;
			this.OutWeight = 0;
		}

		public void Read(AssetReader reader)
		{
			Time = reader.ReadSingle();
			Value = reader.ReadSingle();
			InSlope = reader.ReadSingle();
			OutSlope = reader.ReadSingle();

			//TangentMode = reader.ReadInt32();
			WeightedMode = reader.ReadInt32();
			InWeight = reader.ReadSingle();
			OutWeight = reader.ReadSingle();
		}
		public void Write(AssetWriter writer)
		{
			writer.Write(Time);
			writer.Write(Value);
			writer.Write(InSlope);
			writer.Write(OutSlope);
			//writer.Write(TangentMode);
			writer.Write(WeightedMode);
			writer.Write(InWeight);
			writer.Write(OutWeight);
		}

		public YAMLNode ExportYAML(IExportContainer container)
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Add(TimeName, Time);
			node.Add(ValueName, Value);
			node.Add(InSlopeName, InSlope);
			node.Add(OutSlopeName, OutSlope);
			//node.Add(TangentModeName, TangentMode);
			node.Add(WeightedModeName, WeightedMode);
			node.Add(InWeightName, InWeight);
			node.Add(OutWeightName, OutWeight);
			return node;
		}

		public float Time { get; set; }
		public float Value { get; set; }
		public float InSlope { get; set; }
		public float OutSlope { get; set; }
		public int TangentMode { get; set; }
		public int WeightedMode { get; set; }
		public float InWeight { get; set; }
		public float OutWeight { get; set; }

		public const string TimeName = "time";
		public const string ValueName = "value";
		public const string InSlopeName = "inSlope";
		public const string OutSlopeName = "outSlope";
		public const string TangentModeName = "tangentMode";
		public const string WeightedModeName = "weightedMode";
		public const string InWeightName = "inWeight";
		public const string OutWeightName = "outWeight";
	}
	public struct AnimationCurve : IAssetReadable, IAssetWritable, IYAMLExportable
	{
		public AnimationCurve()
		{
			this.Curve = new Keyframe[0];
			this.PreInfinity = 0;
			this.PostInfinity = 0;
			this.RotationOrder = 0;
		}

		public void Read(AssetReader reader)
		{
			Curve = reader.ReadAssetArray<Keyframe>(false);
			PreInfinity = reader.ReadInt32();
			PostInfinity = reader.ReadInt32();
			RotationOrder = reader.ReadInt32();
		}
		public void Write(AssetWriter writer)
		{
			writer.WriteAssetArray(Curve);
			writer.Write(PreInfinity);
			writer.Write(PostInfinity);
			writer.Write(RotationOrder);
		}

		public YAMLNode ExportYAML(IExportContainer container)
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Add(CurveName, Curve.ExportYAML(container));
			node.Add(PreInfinityName, PreInfinity);
			node.Add(PostInfinityName, PostInfinity);
			node.Add(RotationOrderName, RotationOrder);
			return node;
		}

		public Keyframe[] Curve { get; set; }
		public int PreInfinity { get; set; }
		public int PostInfinity { get; set; }
		public int RotationOrder { get; set; }

		public const string CurveName = "m_Curve";
		public const string PreInfinityName = "m_PreInfinity";
		public const string PostInfinityName = "m_PostInfinity";
		public const string RotationOrderName = "m_RotationOrder";
	}

	public sealed class AudioLowPassFilter : Behaviour
	{
		public AudioLowPassFilter(LayoutInfo layout) : base(layout) { }
		public AudioLowPassFilter(AssetInfo assetInfo) : base(assetInfo) { }

		public override void Read(AssetReader reader)
		{
			base.Read(reader);

			//CutoffFrequency = reader.ReadSingle();
			HighpassResonanceQ = reader.ReadSingle();
			LowpassLevelCustomCurve = reader.ReadAsset<AnimationCurve>();
		}

		public override void Write(AssetWriter writer)
		{
			base.Write(writer);
			writer.Write(HighpassResonanceQ);
			LowpassLevelCustomCurve.Write(writer);
		}

		protected override YAMLMappingNode ExportYAMLRoot(IExportContainer container)
		{
			YAMLMappingNode node = base.ExportYAMLRoot(container);
			//node.Add(CutoffFrequencyName, CutoffFrequency);
			node.Add(HighpassResonanceQName, HighpassResonanceQ);
			node.Add(LowpassLevelCustomCurveName, LowpassLevelCustomCurve.ExportYAML(container));
			return node;
		}

		public float CutoffFrequency { get; set; }
		public float HighpassResonanceQ { get; set; }

		public AnimationCurve LowpassLevelCustomCurve { get; set; }

		public const string CutoffFrequencyName = "m_CutoffFrequency";
		public const string HighpassResonanceQName = "m_HighpassResonanceQ";
		public const string LowpassLevelCustomCurveName = "lowpassLevelCustomCurve";
	}
}
