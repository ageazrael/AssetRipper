using AssetRipper.Core.IO.Asset;
using AssetRipper.Core.IO.Extensions;
using AssetRipper.Core.Layout;
using AssetRipper.Core.Math;
using AssetRipper.Core.Parser.Asset;
using AssetRipper.Core.Project;
using AssetRipper.Core.YAML;

namespace AssetRipper.Core.Classes
{
	public sealed class FixedJoint : Joint
	{
		public FixedJoint(LayoutInfo layout) : base(layout)
		{
		}

		public FixedJoint(AssetInfo assetInfo) : base(assetInfo) { }

		public override void Read(AssetReader reader)
		{
			base.Read(reader);

			ReadJoinPost(reader);
		}

		public override void Write(AssetWriter writer)
		{
			base.Write(writer);

			WriteJoinPost(writer);
		}

		protected override YAMLMappingNode ExportYAMLRoot(IExportContainer container)
		{
			YAMLMappingNode node = base.ExportYAMLRoot(container);

			ExportYAMLRootPost(node, container);
			return node;
		}
	}
}
