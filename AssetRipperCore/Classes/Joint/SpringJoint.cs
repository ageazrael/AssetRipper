using AssetRipper.Core.IO.Asset;
using AssetRipper.Core.IO.Extensions;
using AssetRipper.Core.Layout;
using AssetRipper.Core.Math;
using AssetRipper.Core.Parser.Asset;
using AssetRipper.Core.Project;
using AssetRipper.Core.YAML;

namespace AssetRipper.Core.Classes
{
	public sealed class SpringJoint : Joint
	{
		public SpringJoint(LayoutInfo layout) : base(layout)
		{
		}

		public SpringJoint(AssetInfo assetInfo) : base(assetInfo) { }

		public override void Read(AssetReader reader)
		{
			base.Read(reader);

			ReadJoinPrev(reader, false);

			Spring = reader.ReadSingle();
			Damper = reader.ReadSingle();
			MinDistance = reader.ReadSingle();
			MaxDistance = reader.ReadSingle();
			Tolerance = reader.ReadSingle();

			ReadJoinPost(reader);
		}

		public override void Write(AssetWriter writer)
		{
			base.Write(writer);

			WriteJoinPrev(writer, false);

			writer.Write(Spring);
			writer.Write(Damper);
			writer.Write(MinDistance);
			writer.Write(MaxDistance);
			writer.Write(Tolerance);

			WriteJoinPost(writer);
		}

		protected override YAMLMappingNode ExportYAMLRoot(IExportContainer container)
		{
			YAMLMappingNode node = base.ExportYAMLRoot(container);

			ExportYAMLRootPrev(node, container, false);

			node.AddSerializedVersion(ToSerializedVersion(container.ExportVersion));
			node.Add(SpringName, this.Spring);
			node.Add(DamperName, this.Damper);
			node.Add(MinDistanceName, this.MinDistance);
			node.Add(MaxDistanceName, this.MaxDistance);
			node.Add(ToleranceName, this.Tolerance);

			ExportYAMLRootPost(node, container);
			return node;
		}

		public float Spring { get; set; }
		public float Damper { get; set; }
		public float MinDistance { get; set; }
		public float MaxDistance { get; set; }
		public float Tolerance { get; set; }

		public const string SpringName		= "m_Spring";
		public const string DamperName		= "m_Damper";
		public const string MinDistanceName = "m_MinDistance";
		public const string MaxDistanceName	= "m_MaxDistance";
		public const string ToleranceName	= "m_Tolerance";
	}
}
