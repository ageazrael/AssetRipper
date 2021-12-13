using AssetRipper.Core.IO.Asset;
using AssetRipper.Core.IO.Extensions;
using AssetRipper.Core.Layout;
using AssetRipper.Core.Math;
using AssetRipper.Core.Parser.Asset;
using AssetRipper.Core.Project;
using AssetRipper.Core.YAML;

namespace AssetRipper.Core.Classes
{
	public struct JointSpring : IAssetReadable, IAssetWritable, IYAMLExportable
	{
		public JointSpring()
		{
			this.Spring = 0;
			this.Damper = 0;
		}

		public void Read(AssetReader reader)
		{
			Spring = reader.ReadSingle();
			Damper = reader.ReadSingle();
		}
		public void Write(AssetWriter writer)
		{
			writer.Write(Spring);
			writer.Write(Damper);
		}

		public YAMLNode ExportYAML(IExportContainer container)
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Add(SpringName, Spring);
			node.Add(DamperName, Damper);
			return node;
		}

		public float Spring { get; set; }
		public float Damper { get; set; }

		public const string SpringName = "spring";
		public const string DamperName = "damper";
	}

	public struct JointLimit : IAssetReadable, IAssetWritable, IYAMLExportable
	{
		public JointLimit()
		{
			this.Limit = 0;
			this.Bounciness = 0;
			this.ContactDistance = 0;
		}

		public void Read(AssetReader reader)
		{
			Limit = reader.ReadSingle();
			Bounciness = reader.ReadSingle();
			ContactDistance = reader.ReadSingle();
		}
		public void Write(AssetWriter writer)
		{
			writer.Write(Limit);
			writer.Write(Bounciness);
			writer.Write(ContactDistance);
		}

		public YAMLNode ExportYAML(IExportContainer container)
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Add(LimitName, Limit);
			node.Add(BouncinessName, Bounciness);
			node.Add(BouncinessName, ContactDistance);
			return node;
		}

		public float Limit { get; set; }
		public float Bounciness { get; set; }
		public float ContactDistance { get; set; }

		public const string LimitName = "limit";
		public const string BouncinessName = "bounciness";
		public const string ContactDistanceName = "contactDistance";
	}

	public sealed class CharacterJoint : Joint
	{
		public CharacterJoint(LayoutInfo layout) : base(layout)
		{
		}

		public CharacterJoint(AssetInfo assetInfo) : base(assetInfo) { }

		public override void Read(AssetReader reader)
		{
			base.Read(reader);

			ReadJoinPrev(reader);

			SwingAxis.Read(reader);
			TwistLimitSpring.Read(reader);
			LowTwistLimit.Read(reader);
			HighTwistLimit.Read(reader);
			SwingLimitSpring.Read(reader);
			Swing1Limit.Read(reader);
			Swing2Limit.Read(reader);

			EnableProjection = reader.ReadByte();
			if (IsAlign(reader.Version))
				reader.AlignStream();
			ProjectionDistance = reader.ReadSingle();
			ProjectionAngle = reader.ReadSingle();

			ReadJoinPost(reader);
		}

		public override void Write(AssetWriter writer)
		{
			base.Write(writer);

			WriteJoinPrev(writer);

			SwingAxis.Write(writer);
			TwistLimitSpring.Write(writer);
			LowTwistLimit.Write(writer);
			HighTwistLimit.Write(writer);
			SwingLimitSpring.Write(writer);
			Swing1Limit.Write(writer);
			Swing2Limit.Write(writer);

			writer.Write(EnableProjection);
			if (IsAlign(writer.Version))
				writer.AlignStream();
			writer.Write(ProjectionDistance);
			writer.Write(ProjectionAngle);

			WriteJoinPost(writer);
		}

		protected override YAMLMappingNode ExportYAMLRoot(IExportContainer container)
		{
			YAMLMappingNode node = base.ExportYAMLRoot(container);

			ExportYAMLRootPrev(node, container);

			node.AddSerializedVersion(ToSerializedVersion(container.ExportVersion));
			node.Add(SwingAxisName, SwingAxis.ExportYAML(container));
			node.Add(TwistLimitSpringName, TwistLimitSpring.ExportYAML(container));
			node.Add(LowTwistLimitName, LowTwistLimit.ExportYAML(container));
			node.Add(HighTwistLimitName, HighTwistLimit.ExportYAML(container));
			node.Add(SwingLimitSpringName, SwingLimitSpring.ExportYAML(container));
			node.Add(Swing1LimitName, Swing1Limit.ExportYAML(container));
			node.Add(Swing2LimitName, Swing2Limit.ExportYAML(container));

			node.Add(EnableProjectionName, EnableProjection);
			node.Add(ProjectionDistanceName, ProjectionDistance);
			node.Add(ProjectionAngleName, ProjectionAngle);

			ExportYAMLRootPost(node, container);
			return node;
		}

		public Vector3f SwingAxis { get; set; }
		public JointSpring TwistLimitSpring { get; set; }
		public JointLimit LowTwistLimit { get; set; }
		public JointLimit HighTwistLimit { get; set; }
		public JointSpring SwingLimitSpring { get; set; }
		public JointLimit Swing1Limit { get; set; }
		public JointLimit Swing2Limit { get; set; }

		public byte EnableProjection { get; set; }
		public float ProjectionDistance { get; set; }
		public float ProjectionAngle { get; set; }

		public const string SwingAxisName = "m_SwingAxis";
		public const string TwistLimitSpringName = "m_TwistLimitSpring";
		public const string LowTwistLimitName = "m_LowTwistLimit";
		public const string HighTwistLimitName = "m_HighTwistLimit";
		public const string SwingLimitSpringName = "m_SwingLimitSpring";
		public const string Swing1LimitName = "m_Swing1Limit";
		public const string Swing2LimitName = "m_Swing2Limit";

		public const string EnableProjectionName = "m_EnableProjection";
		public const string ProjectionDistanceName = "m_ProjectionDistance";
		public const string ProjectionAngleName = "m_ProjectionAngle";
	}
}
