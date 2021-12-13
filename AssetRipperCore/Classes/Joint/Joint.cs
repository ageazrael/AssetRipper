using AssetRipper.Core.Classes.Misc;
using AssetRipper.Core.Interfaces;
using AssetRipper.Core.IO.Asset;
using AssetRipper.Core.IO.Extensions;
using AssetRipper.Core.Layout;
using AssetRipper.Core.Math;
using AssetRipper.Core.Parser.Asset;
using AssetRipper.Core.Parser.Files;
using AssetRipper.Core.Project;
using AssetRipper.Core.YAML;
using System.Collections.Generic;

namespace AssetRipper.Core.Classes
{

	public class Joint : Component
	{
		public Joint(LayoutInfo layout) : base(layout)
		{
		}

		public Joint(AssetInfo assetInfo) : base(assetInfo) { }

		public override void Read(AssetReader reader)
		{
			base.Read(reader);

			ConnectedBody.Read(reader);
		}

		public override void Write(AssetWriter writer)
		{
			base.Write(writer);

			ConnectedBody.Write(writer);
		}

		protected void ReadJoinPrev(AssetReader reader, bool bAllowAxis = true)
		{
			Anchor.Read(reader);
			if (bAllowAxis)
				Axis.Read(reader);
			AutoConfigureConnectedAnchor = reader.ReadByte();
			reader.AlignStream();
			ConnectedAnchor.Read(reader);
		}
		protected void ReadJoinPost(AssetReader reader)
		{
			BreakForce = reader.ReadSingle();
			BreakTorque = reader.ReadSingle();
			EnableCollision = reader.ReadByte();
			EnablePreprocessing = reader.ReadByte();
			if (IsAlign(reader.Version))
				reader.AlignStream();
			if (HasMassScale(reader.Version))
			{
				MassScale = reader.ReadSingle();
				ConnectedMassScale = reader.ReadSingle();
			}
		}
		protected void WriteJoinPrev(AssetWriter writer, bool bAllowAxis = true)
		{
			Anchor.Write(writer);
			if (bAllowAxis)
				Axis.Write(writer);
			writer.Write(AutoConfigureConnectedAnchor);
			writer.AlignStream();
			ConnectedAnchor.Write(writer);
		}
		protected void WriteJoinPost(AssetWriter writer)
		{
			writer.Write(BreakForce);
			writer.Write(BreakTorque);
			writer.Write(EnableCollision);
			writer.Write(EnablePreprocessing);
			if (IsAlign(writer.Version))
				writer.AlignStream();
			if (HasMassScale(writer.Version))
			{
				writer.Write(MassScale);
				writer.Write(ConnectedMassScale);
			}
		}

		public override IEnumerable<PPtr<IUnityObjectBase>> FetchDependencies(DependencyContext context)
		{
			foreach (PPtr<IUnityObjectBase> asset in base.FetchDependencies(context))
			{
				yield return asset;
			}

			yield return context.FetchDependency(ConnectedBody, ConnectedBodyName);
		}

		protected override YAMLMappingNode ExportYAMLRoot(IExportContainer container)
		{
			YAMLMappingNode node = base.ExportYAMLRoot(container);

			node.Add(ConnectedBodyName, ConnectedBody.ExportYAML(container));
			
			return node;
		}

		protected void ExportYAMLRootPrev(YAMLMappingNode node, IExportContainer container, bool bAllowAxis = true)
		{
			node.Add(AnchorName, Anchor.ExportYAML(container));
			if (bAllowAxis)
				node.Add(AxisName, Axis.ExportYAML(container));
			node.Add(AutoConfigureConnectedAnchorName, AutoConfigureConnectedAnchor);
			node.Add(ConnectedAnchorName, ConnectedAnchor.ExportYAML(container));
		}
		protected void ExportYAMLRootPost(YAMLMappingNode node, IExportContainer container)
		{
			node.Add(BreakForceName, BreakForce);
			node.Add(BreakTorqueName, BreakTorque);
			node.Add(EnableCollisionName, EnableCollision);
			node.Add(EnablePreprocessingName, EnablePreprocessing);
			if (HasMassScale(container.ExportVersion))
			{
				node.Add(MassScaleName, MassScale);
				node.Add(ConnectedMassScaleName, ConnectedMassScale);
			}
		}

		/// <summary>
		/// 2.1.0 and greater
		/// </summary>
		public static bool IsAlign(UnityVersion version) => version.IsGreaterEqual(2, 1);
		public static int ToSerializedVersion(UnityVersion version)
		{
			if (version.IsGreaterEqual(5))
			{
				return 2;
			}
			return 1;
		}

		public static bool HasMassScale(UnityVersion version) => ToSerializedVersion(version) >= 2;

		public PPtr<Rigidbody.Rigidbody> ConnectedBody { get; set; }
		public Vector3f Anchor { get; set; }
		public Vector3f Axis { get; set; }
		public byte AutoConfigureConnectedAnchor { get; set; }
		public Vector3f ConnectedAnchor { get; set; }

		public float BreakForce { get; set; }
		public float BreakTorque { get; set; }
		public byte EnableCollision { get; set; }
		public byte EnablePreprocessing { get; set; }
		public float MassScale { get; set; }
		public float ConnectedMassScale { get; set; }


		public const string ConnectedBodyName = "m_ConnectedBody";
		public const string AnchorName = "m_Anchor";
		public const string AxisName = "m_Axis";
		public const string AutoConfigureConnectedAnchorName = "m_AutoConfigureConnectedAnchor";
		public const string ConnectedAnchorName = "m_ConnectedAnchor";


		public const string BreakForceName = "m_BreakForce";
		public const string BreakTorqueName = "m_BreakTorque";
		public const string EnableCollisionName = "m_EnableCollision";
		public const string EnablePreprocessingName = "m_EnablePreprocessing";
		public const string MassScaleName = "m_MassScale";
		public const string ConnectedMassScaleName = "m_ConnectedMassScale";
	}
}
