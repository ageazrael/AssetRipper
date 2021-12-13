using AssetRipper.Core.IO.Asset;
using AssetRipper.Core.IO.Extensions;
using AssetRipper.Core.Layout;
using AssetRipper.Core.Math;
using AssetRipper.Core.Parser.Asset;
using AssetRipper.Core.Project;
using AssetRipper.Core.YAML;

namespace AssetRipper.Core.Classes
{
	public struct HingeJointSpring : IAssetReadable, IAssetWritable, IYAMLExportable
	{
		public HingeJointSpring()
		{
			this.Spring = 0;
			this.Damper = 0;
			this.TargetPosition = 0;
		}

		public void Read(AssetReader reader)
		{
			Spring = reader.ReadSingle();
			Damper = reader.ReadSingle();
			TargetPosition = reader.ReadSingle();
		}
		public void Write(AssetWriter writer)
		{
			writer.Write(Spring);
			writer.Write(Damper);
			writer.Write(TargetPosition);
		}

		public YAMLNode ExportYAML(IExportContainer container)
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Add(SpringName, Spring);
			node.Add(DamperName, Damper);
			node.Add(TargetPositionName, TargetPosition);
			return node;
		}

		public float Spring { get; set; }
		public float Damper { get; set; }
		public float TargetPosition { get; set; }

		public const string SpringName = "spring";
		public const string DamperName = "damper";
		public const string TargetPositionName = "targetPosition";
	}

	public struct HingeJointMotor : IAssetReadable, IAssetWritable, IYAMLExportable
	{
		public HingeJointMotor()
		{
			this.TargetVelocity = 0;
			this.Force = 0;
			this.FreeSpin = 0;
		}

		public void Read(AssetReader reader)
		{
			TargetVelocity = reader.ReadSingle();
			Force = reader.ReadSingle();
			FreeSpin = reader.ReadSingle();
		}
		public void Write(AssetWriter writer)
		{
			writer.Write(TargetVelocity);
			writer.Write(Force);
			writer.Write(FreeSpin);
		}

		public YAMLNode ExportYAML(IExportContainer container)
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Add(TargetVelocityName, TargetVelocity);
			node.Add(ForceName, Force);
			node.Add(FreeSpinName, FreeSpin);
			return node;
		}

		public float TargetVelocity { get; set; }
		public float Force { get; set; }
		public float FreeSpin { get; set; }

		public const string TargetVelocityName = "targetVelocity";
		public const string ForceName = "force";
		public const string FreeSpinName = "freeSpin";
	}

	public struct HingeJointLimits : IAssetReadable, IAssetWritable, IYAMLExportable
	{
		public HingeJointLimits()
		{
			this.Min = 0;
			this.Max = 0;
			this.Bounciness = 0;
			this.BounceMinVelocity = 0;
			this.ContactDistance = 0;
		}

		public void Read(AssetReader reader)
		{
			Min = reader.ReadSingle();
			Max = reader.ReadSingle();
			Bounciness = reader.ReadSingle();
			BounceMinVelocity = reader.ReadSingle();
			ContactDistance = reader.ReadSingle();
		}
		public void Write(AssetWriter writer)
		{
			writer.Write(Min);
			writer.Write(Max);
			writer.Write(Bounciness);
			writer.Write(BounceMinVelocity);
			writer.Write(ContactDistance);
		}

		public YAMLNode ExportYAML(IExportContainer container)
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Add(MinName, Min);
			node.Add(MaxName, Max);
			node.Add(BouncinessName, Bounciness);
			node.Add(BounceMinVelocityName, BounceMinVelocity);
			node.Add(ContactDistanceName, ContactDistance);
			return node;
		}

		public float Min { get; set; }
		public float Max { get; set; }
		public float Bounciness { get; set; }
		public float BounceMinVelocity { get; set; }
		public float ContactDistance { get; set; }

		public const string MinName = "min";
		public const string MaxName = "max";
		public const string BouncinessName = "bounciness";
		public const string BounceMinVelocityName = "bounceMinVelocity";
		public const string ContactDistanceName = "contactDistance";
	}

	public sealed class HingeJoint : Joint
	{
		public HingeJoint(LayoutInfo layout) : base(layout)
		{
		}

		public HingeJoint(AssetInfo assetInfo) : base(assetInfo) { }

		public override void Read(AssetReader reader)
		{
			base.Read(reader);

			ReadJoinPrev(reader);

			UseSpring = reader.ReadByte();
			reader.AlignStream();
			Spring.Read(reader);

			UseMotor = reader.ReadByte();
			reader.AlignStream();
			Motor.Read(reader);

			UseLimits = reader.ReadByte();
			reader.AlignStream();
			Limits.Read(reader);

			ReadJoinPost(reader);
		}

		public override void Write(AssetWriter writer)
		{
			base.Write(writer);

			WriteJoinPrev(writer);

			writer.Write(UseSpring);
			writer.AlignStream();
			Spring.Write(writer);

			writer.Write(UseMotor);
			writer.AlignStream();
			Motor.Write(writer);

			writer.Write(UseLimits);
			writer.AlignStream();
			Limits.Write(writer);

			WriteJoinPost(writer);
		}

		protected override YAMLMappingNode ExportYAMLRoot(IExportContainer container)
		{
			YAMLMappingNode node = base.ExportYAMLRoot(container);

			ExportYAMLRootPrev(node, container);
			node.Add(UseSpringName, UseSpring);
			node.Add(SpringName, Spring.ExportYAML(container));
			node.Add(UseMotorName, UseMotor);
			node.Add(MotorName, Motor.ExportYAML(container));
			node.Add(UseLimitsName, UseLimits);
			node.Add(LimitsName, Limits.ExportYAML(container));
			ExportYAMLRootPost(node, container);
			return node;
		}

		public byte UseSpring { get; set; }
		public HingeJointSpring Spring { get; set; }
		public byte UseMotor { get; set; }
		public HingeJointMotor Motor { get; set; }
		public byte UseLimits { get; set; }
		public HingeJointLimits Limits { get; set; }


		public const string UseSpringName = "m_UseSpring";
		public const string SpringName = "m_Spring";
		public const string UseMotorName = "m_UseMotor";
		public const string MotorName = "m_Motor";
		public const string UseLimitsName = "m_UseLimits";
		public const string LimitsName = "m_Limits";
	}
}
