using AssetRipper.Core.IO.Asset;
using AssetRipper.Core.IO.Extensions;
using AssetRipper.Core.Layout;
using AssetRipper.Core.Math;
using AssetRipper.Core.Parser.Asset;
using AssetRipper.Core.Project;
using AssetRipper.Core.YAML;

namespace AssetRipper.Core.Classes
{
	public struct JointDrive : IAssetReadable, IAssetWritable, IYAMLExportable
	{
		public JointDrive()
		{
			this.PositionSpring = 0;
			this.PositionDamper = 0;
			this.MaximumForce = 0;
		}

		public void Read(AssetReader reader)
		{
			PositionSpring = reader.ReadSingle();
			PositionDamper = reader.ReadSingle();
			MaximumForce = reader.ReadSingle();
		}
		public void Write(AssetWriter writer)
		{
			writer.Write(PositionSpring);
			writer.Write(PositionDamper);
			writer.Write(MaximumForce);
		}

		public YAMLNode ExportYAML(IExportContainer container)
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.AddSerializedVersion(3); // TODO: ???
			node.Add(PositionSpringName, PositionSpring);
			node.Add(PositionDamperName, PositionDamper);
			node.Add(PositionDamperName, MaximumForce);
			return node;
		}

		public float PositionSpring { get; set; }
		public float PositionDamper { get; set; }
		public float MaximumForce { get; set; }

		public const string PositionSpringName = "positionSpring";
		public const string PositionDamperName = "positionDamper";
		public const string MaximumForceName = "maximumForce";
	}
	public sealed class ConfigurableJoint : Joint
	{
		public ConfigurableJoint(LayoutInfo layout) : base(layout)
		{
		}

		public ConfigurableJoint(AssetInfo assetInfo) : base(assetInfo) { }

		public override void Read(AssetReader reader)
		{
			base.Read(reader);

			ReadJoinPrev(reader, true);

			SecondaryAxis.Read(reader);

			XMotion = reader.ReadSingle();
			YMotion = reader.ReadSingle();
			ZMotion = reader.ReadSingle();

			AngularXMotion = reader.ReadSingle();
			AngularYMotion = reader.ReadSingle();
			AngularZMotion = reader.ReadSingle();

			LinearLimitSpring.Read(reader);
			LinearLimit.Read(reader);
			AngularXLimitSpring.Read(reader);
			LowAngularXLimit.Read(reader);
			HighAngularXLimit.Read(reader);
			AngularYZLimitSpring.Read(reader);
			AngularYLimit.Read(reader);
			AngularZLimit.Read(reader);
			TargetPosition.Read(reader);
			TargetVelocity.Read(reader);
			XDrive.Read(reader);
			YDrive.Read(reader);
			ZDrive.Read(reader);
			TargetRotation.Read(reader);
			TargetAngularVelocity.Read(reader);

			RotationDriveMode = reader.ReadInt32();
			AngularXDrive.Read(reader);
			AngularYZDrive.Read(reader);
			SlerpDrive.Read(reader);

			ProjectionMode = reader.ReadInt32();
			ProjectionDistance = reader.ReadSingle();
			ProjectionAngle = reader.ReadSingle();

			ConfiguredInWorldSpace = reader.ReadByte();
			SwapBodies = reader.ReadByte();

			reader.AlignStream();

			ReadJoinPost(reader);
		}

		public override void Write(AssetWriter writer)
		{
			base.Write(writer);

			WriteJoinPrev(writer, true);

			SecondaryAxis.Write(writer);

			writer.Write(XMotion);
			writer.Write(YMotion);
			writer.Write(ZMotion);

			writer.Write(AngularXMotion);
			writer.Write(AngularYMotion);
			writer.Write(AngularZMotion);

			LinearLimitSpring.Write(writer);
			LinearLimit.Write(writer);
			AngularXLimitSpring.Write(writer);
			LowAngularXLimit.Write(writer);
			HighAngularXLimit.Write(writer);
			AngularYZLimitSpring.Write(writer);
			AngularYLimit.Write(writer);
			AngularZLimit.Write(writer);
			TargetPosition.Write(writer);
			TargetVelocity.Write(writer);
			XDrive.Write(writer);
			YDrive.Write(writer);
			ZDrive.Write(writer);
			TargetRotation.Write(writer);
			TargetAngularVelocity.Write(writer);

			writer.Write(RotationDriveMode);
			AngularXDrive.Write(writer);
			AngularYZDrive.Write(writer);
			SlerpDrive.Write(writer);

			writer.Write(ProjectionMode);
			writer.Write(ProjectionDistance);
			writer.Write(ProjectionAngle);

			writer.Write(ConfiguredInWorldSpace);
			writer.Write(SwapBodies);

			writer.AlignStream();

			WriteJoinPost(writer);
		}

		protected override YAMLMappingNode ExportYAMLRoot(IExportContainer container)
		{
			YAMLMappingNode node = base.ExportYAMLRoot(container);

			ExportYAMLRootPrev(node, container, true);

			node.AddSerializedVersion(ToSerializedVersion(container.ExportVersion));
			node.Add(SecondaryAxisName, SecondaryAxis.ExportYAML(container));

			node.Add(XMotionName, XMotion);
			node.Add(YMotionName, YMotion);
			node.Add(ZMotionName, ZMotion);

			node.Add(AngularXMotionName, AngularXMotion);
			node.Add(LinearLimitSpringName, LinearLimitSpring.ExportYAML(container));
			node.Add(LinearLimitName, LinearLimit.ExportYAML(container));
			node.Add(AngularXLimitSpringName, AngularXLimitSpring.ExportYAML(container));
			node.Add(LowAngularXLimitName, LowAngularXLimit.ExportYAML(container));
			node.Add(HighAngularXLimitName, HighAngularXLimit.ExportYAML(container));
			node.Add(AngularYZLimitSpringName, AngularYZLimitSpring.ExportYAML(container));
			node.Add(AngularYLimitName, AngularYLimit.ExportYAML(container));
			node.Add(AngularZLimitName, AngularZLimit.ExportYAML(container));
			node.Add(TargetPositionName, TargetPosition.ExportYAML(container));
			node.Add(TargetVelocityName, TargetVelocity.ExportYAML(container));
			node.Add(XDriveName, XDrive.ExportYAML(container));
			node.Add(YDriveName, YDrive.ExportYAML(container));
			node.Add(ZDriveName, ZDrive.ExportYAML(container));
			node.Add(TargetRotationName, TargetRotation.ExportYAML(container));
			node.Add(TargetAngularVelocityName, TargetAngularVelocity.ExportYAML(container));

			node.Add(RotationDriveModeName, RotationDriveMode);
			node.Add(AngularXDriveName, AngularXDrive.ExportYAML(container));
			node.Add(AngularYZDriveName, AngularYZDrive.ExportYAML(container));
			node.Add(SlerpDriveName, SlerpDrive.ExportYAML(container));

			node.Add(ProjectionModeName, ProjectionMode);
			node.Add(ProjectionDistanceName, ProjectionDistance);
			node.Add(ProjectionAngleName, ProjectionAngle);

			node.Add(ConfiguredInWorldSpaceName, ConfiguredInWorldSpace);
			node.Add(SwapBodiesName, SwapBodies);

			ExportYAMLRootPost(node, container);
			return node;
		}

		public Vector3f SecondaryAxis { get; set; }

		public float XMotion { get; set; }
		public float YMotion { get; set; }
		public float ZMotion { get; set; }
		public float AngularXMotion { get; set; }
		public float AngularYMotion { get; set; }
		public float AngularZMotion { get; set; }
		public JointSpring LinearLimitSpring { get; set; }
		public JointLimit LinearLimit { get; set; }
		public JointSpring AngularXLimitSpring { get; set; }
		public JointLimit LowAngularXLimit { get; set; }
		public JointLimit HighAngularXLimit { get; set; }
		public JointSpring AngularYZLimitSpring { get; set; }
		public JointLimit AngularYLimit { get; set; }
		public JointLimit AngularZLimit { get; set; }
		public Vector3f TargetPosition { get; set; }
		public Vector3f TargetVelocity { get; set; }
		public JointDrive XDrive { get; set; }
		public JointDrive YDrive { get; set; }
		public JointDrive ZDrive { get; set; }
		public Quaternionf TargetRotation { get; set; }
		public Vector3f TargetAngularVelocity { get; set; }
		public int RotationDriveMode { get; set; }
		public JointDrive AngularXDrive { get; set; }
		public JointDrive AngularYZDrive { get; set; }
		public JointDrive SlerpDrive { get; set; }
		public int ProjectionMode { get; set; }
		public float ProjectionDistance { get; set; }
		public float ProjectionAngle { get; set; }
		public byte ConfiguredInWorldSpace { get; set; }
		public byte SwapBodies { get; set; }

		public const string SecondaryAxisName = "m_SecondaryAxis";
		public const string XMotionName = "m_XMotion";
		public const string YMotionName = "m_YMotion";
		public const string ZMotionName = "m_ZMotion";
		public const string AngularXMotionName = "m_AngularXMotion";
		public const string AngularYMotionName = "m_AngularYMotion";
		public const string AngularZMotionName = "m_AngularZMotion";
		public const string LinearLimitSpringName = "m_LinearLimitSpring";
		public const string LinearLimitName = "m_LinearLimit";
		public const string AngularXLimitSpringName = "m_AngularXLimitSpring";
		public const string LowAngularXLimitName = "m_LowAngularXLimit";
		public const string HighAngularXLimitName = "m_HighAngularXLimit";
		public const string AngularYZLimitSpringName = "m_AngularYZLimitSpring";
		public const string AngularYLimitName = "m_AngularYLimit";
		public const string AngularZLimitName = "m_AngularZLimit";
		public const string TargetPositionName = "m_TargetPosition";
		public const string TargetVelocityName = "m_TargetVelocity";
		public const string XDriveName = "m_XDrive";
		public const string YDriveName = "m_YDrive";
		public const string ZDriveName = "m_ZDrive";
		public const string TargetRotationName = "m_TargetRotation";
		public const string TargetAngularVelocityName = "m_TargetAngularVelocity";
		public const string RotationDriveModeName = "m_RotationDriveMode";
		public const string AngularXDriveName = "m_AngularXDrive";
		public const string AngularYZDriveName = "m_AngularYZDrive";
		public const string SlerpDriveName = "m_SlerpDrive";
		public const string ProjectionModeName = "m_ProjectionMode";
		public const string ProjectionDistanceName = "m_ProjectionDistance";
		public const string ProjectionAngleName = "m_ProjectionAngle";
		public const string ConfiguredInWorldSpaceName = "m_ConfiguredInWorldSpace";
		public const string SwapBodiesName = "m_SwapBodies";
	}
}
