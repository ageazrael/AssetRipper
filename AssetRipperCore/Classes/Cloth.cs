using AssetRipper.Core.IO.Asset;
using AssetRipper.Core.IO.Extensions;
using AssetRipper.Core.Layout;
using AssetRipper.Core.Math;
using AssetRipper.Core.Parser.Asset;
using AssetRipper.Core.Project;
using AssetRipper.Core.Classes;
using AssetRipper.Core.Classes.Misc;
using AssetRipper.Core.YAML;
using AssetRipper.Core.YAML.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AssetRipper.Core.Classes
{
	public struct ClothCoefficients : IAssetReadable, IAssetWritable, IYAMLExportable
	{
		public ClothCoefficients()
		{
			this.MaxDistance = 0;
			this.CollisionSphereDistance = 0;
		}

		public void Read(AssetReader reader)
		{
			MaxDistance = reader.ReadSingle();
			CollisionSphereDistance = reader.ReadSingle();
		}
		public void Write(AssetWriter writer)
		{
			writer.Write(MaxDistance);
			writer.Write(CollisionSphereDistance);
		}

		public YAMLNode ExportYAML(IExportContainer container)
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Add(MaxDistanceName, MaxDistance);
			node.Add(CollisionSphereDistanceName, CollisionSphereDistance);
			return node;
		}

		public float MaxDistance { get; set; }
		public float CollisionSphereDistance { get; set; }

		public const string MaxDistanceName = "maxDistance";
		public const string CollisionSphereDistanceName = "collisionSphereDistance";
	}
	public struct ClothSphereColliders : IAssetReadable, IAssetWritable, IYAMLExportable
	{
		public ClothSphereColliders()
		{
			this.First = new PPtr<SphereCollider>();
			this.Second = new PPtr<SphereCollider>();
		}
		public void Read(AssetReader reader)
		{
			First.Read(reader);
			Second.Read(reader);
		}
		public void Write(AssetWriter writer)
		{
			First.Write(writer);
			Second.Write(writer);
		}
		public YAMLNode ExportYAML(IExportContainer container)
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Add("first", First.ExportYAML(container));
			node.Add("second", Second.ExportYAML(container));
			return node;
		}
		public PPtr<SphereCollider> First { get; set; }
		public PPtr<SphereCollider> Second { get; set; }
	}
	public sealed class Cloth : Behaviour
	{
		public Cloth(LayoutInfo layout) : base(layout) { }
		public Cloth(AssetInfo assetInfo) : base(assetInfo) { }

		public override void Read(AssetReader reader)
		{
			base.Read(reader);

			StretchingStiffness = reader.ReadSingle();
			BendingStiffness = reader.ReadSingle();
			UseTethers = reader.ReadByte();
			UseGravity = reader.ReadByte();
			reader.AlignStream();
			Damping = reader.ReadSingle();

			ExternalAcceleration.Read(reader);
			RandomAcceleration.Read(reader);
			WorldVelocityScale = reader.ReadSingle();
			WorldAccelerationScale = reader.ReadSingle();
			Friction = reader.ReadSingle();

			CollisionMassScale = reader.ReadSingle();
			UseContinuousCollision = reader.ReadByte();
			UseVirtualParticles = reader.ReadByte();
			reader.AlignStream();
			SolverFrequency = reader.ReadSingle();
			SleepThreshold = reader.ReadSingle();

			Coefficients = reader.ReadAssetArray<ClothCoefficients>();
			CapsuleColliders = reader.ReadAssetArray<PPtr<CapsuleCollider>>();
			SphereColliders = reader.ReadAssetArray<ClothSphereColliders>();
			SelfCollisionDistance = reader.ReadSingle();
			SelfCollisionStiffness = reader.ReadSingle();

			SelfAndInterCollisionIndices = reader.ReadInt32Array();
			VirtualParticleWeights = reader.ReadAssetArray<Vector3f>();
			VirtualParticleIndices = reader.ReadInt32Array();
		}

		public override void Write(AssetWriter writer)
		{
			base.Write(writer);

			writer.Write(StretchingStiffness);
			writer.Write(BendingStiffness);
			writer.Write(UseTethers);
			writer.Write(UseGravity);
			writer.AlignStream();
			writer.Write(Damping);

			writer.WriteAsset(ExternalAcceleration);
			writer.WriteAsset(RandomAcceleration);
			writer.Write(WorldVelocityScale);
			writer.Write(WorldAccelerationScale);
			writer.Write(Friction);

			writer.Write(CollisionMassScale);
			writer.Write(UseContinuousCollision);
			writer.Write(UseVirtualParticles);
			writer.AlignStream();
			writer.Write(SolverFrequency);
			writer.Write(SleepThreshold);

			writer.WriteAssetArray(Coefficients);
			writer.WriteAssetArray(CapsuleColliders);
			writer.WriteAssetArray(SphereColliders);
			writer.Write(SelfCollisionDistance);
			writer.Write(SelfCollisionStiffness);
			writer.WriteArray(SelfAndInterCollisionIndices);
			writer.WriteAssetArray(VirtualParticleWeights);
			writer.WriteArray(VirtualParticleIndices);
		}

		protected override YAMLMappingNode ExportYAMLRoot(IExportContainer container)
		{
			YAMLMappingNode node = base.ExportYAMLRoot(container);
			node.Add(StretchingStiffnessName, StretchingStiffness);
			node.Add(BendingStiffnessName, BendingStiffness);
			node.Add(UseTethersName, UseTethers);
			node.Add(UseGravityName, UseGravity);
			node.Add(DampingName, Damping);
			node.Add(ExternalAccelerationName, ExternalAcceleration.ExportYAML(container));
			node.Add(RandomAccelerationName, RandomAcceleration.ExportYAML(container));
			node.Add(WorldVelocityScaleName, WorldVelocityScale);
			node.Add(WorldAccelerationScaleName, WorldAccelerationScale);
			node.Add(FrictionName, Friction);
			node.Add(CollisionMassScaleName, CollisionMassScale);
			node.Add(UseContinuousCollisionName, UseContinuousCollision);
			node.Add(UseVirtualParticlesName, UseVirtualParticles);
			node.Add(SolverFrequencyName, SolverFrequency);
			node.Add(SleepThresholdName, SleepThreshold);
			node.Add(CoefficientsName, Coefficients.ExportYAML(container));
			node.Add(CapsuleCollidersName, CapsuleColliders.ExportYAML(container));
			node.Add(SphereCollidersName, SphereColliders.ExportYAML(container));
			node.Add(SelfCollisionDistanceName, SelfCollisionDistance);
			node.Add(SelfCollisionStiffnessName, SelfCollisionStiffness);
			node.Add(SelfAndInterCollisionIndicesName, SelfAndInterCollisionIndices.ExportYAML(true));
			node.Add(VirtualParticleWeightsName, VirtualParticleWeights.ExportYAML(container));
			node.Add(VirtualParticleIndicesName, VirtualParticleIndices.ExportYAML(true));
			return node;
		}
		public float StretchingStiffness { get; set; }
		public float BendingStiffness { get; set; }
		public byte UseTethers { get; set; }
		public byte UseGravity { get; set; }
		public float Damping { get; set; }
		public Vector3f ExternalAcceleration { get; set; }
		public Vector3f RandomAcceleration { get; set; }
		public float WorldVelocityScale { get; set; }
		public float WorldAccelerationScale { get; set; }
		public float Friction { get; set; }
		public float CollisionMassScale { get; set; }
		public byte UseContinuousCollision { get; set; }
		public byte UseVirtualParticles { get; set; }
		public float SolverFrequency { get; set; }
		public float SleepThreshold { get; set; }
		public ClothCoefficients[] Coefficients { get; set; }
		public PPtr<CapsuleCollider>[] CapsuleColliders { get; set; }
		public ClothSphereColliders[] SphereColliders { get; set; }
		public float SelfCollisionDistance { get; set; }
		public float SelfCollisionStiffness { get; set; }
		public int[] SelfAndInterCollisionIndices { get; set; }
		public Vector3f[] VirtualParticleWeights { get; set; }
		public int[] VirtualParticleIndices { get; set; }

		public const string StretchingStiffnessName = "m_StretchingStiffness";
		public const string BendingStiffnessName = "m_BendingStiffness";
		public const string UseTethersName = "m_UseTethers";
		public const string UseGravityName = "m_UseGravity";
		public const string DampingName = "m_Damping";
		public const string ExternalAccelerationName = "m_ExternalAcceleration";
		public const string RandomAccelerationName = "m_RandomAcceleration";
		public const string WorldVelocityScaleName = "m_WorldVelocityScale";
		public const string WorldAccelerationScaleName = "m_WorldAccelerationScale";
		public const string FrictionName = "m_Friction";
		public const string CollisionMassScaleName = "m_CollisionMassScale";

		public const string UseContinuousCollisionName = "m_UseContinuousCollision";
		public const string UseVirtualParticlesName = "m_UseVirtualParticles";
		public const string SolverFrequencyName = "m_SolverFrequency";
		public const string SleepThresholdName = "m_SleepThreshold";
		public const string CoefficientsName = "m_Coefficients";

		public const string CapsuleCollidersName = "m_CapsuleColliders";
		public const string SphereCollidersName = "m_SphereColliders";

		public const string SelfCollisionDistanceName = "m_SelfCollisionDistance";
		public const string SelfCollisionStiffnessName = "m_SelfCollisionStiffness";
		public const string SelfAndInterCollisionIndicesName = "m_SelfAndInterCollisionIndices";
		public const string VirtualParticleWeightsName = "m_VirtualParticleWeights";
		public const string VirtualParticleIndicesName = "m_VirtualParticleIndices";
	}
}
