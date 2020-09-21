using System.Text;
using System.Xml;
using System.Xml.Linq;
using JetBrains.Annotations;
using asm.Data.Helpers;
using asm.Data.Xml;
using asm.Web;

// ReSharper disable once CheckNamespace
namespace asm.Extensions
{
	public static class XmlNodeExtension
	{
		public static int GetIndex([NotNull] this XmlNode thisValue, XmlIndexMatchType matchType)
		{
			if (thisValue.ParentNode == null) return 0;

			int index = 0;

			if (matchType == XmlIndexMatchType.None)
			{
				for (XmlNode n = thisValue; n != null; n = n.PreviousSibling)
					++index;

				return index;
			}

			bool matchT = matchType.HasFlag(XmlIndexMatchType.Type);
			bool matchN = thisValue.NodeType == XmlNodeType.Element && matchType.HasFlag(XmlIndexMatchType.Name);

			if (matchN)
			{
				for (XmlNode n = thisValue; n != null; n = n.PreviousSibling)
				{
					if (matchT && n.NodeType != thisValue.NodeType) continue;
					if (!n.Name.IsSame(thisValue.Name)) continue;
					++index;
				}
			}
			else
			{
				for (XmlNode n = thisValue; n != null; n = n.PreviousSibling)
				{
					if (matchT && n.NodeType != thisValue.NodeType) continue;
					++index;
				}
			}

			return index;
		}

		public static XNode ToXNode([NotNull] this XmlNode thisValue)
		{
			XNode node;

			using (XNodeBuilder builder = new XNodeBuilder())
			{
				using (XmlWriter writer = XmlWriter.Create(new XNodeBuilder(), XmlWriterHelper.CreateSettings()))
					thisValue.WriteTo(writer);

				node = builder.Root;
			}

			return node;
		}

		public static string GetXPath([NotNull] this XmlNode thisValue)
		{
			StringBuilder sb = new StringBuilder();
			XmlNode node = thisValue;
			
			while (node != null)
			{
				int index;
			
				switch (node.NodeType)
				{
					case XmlNodeType.Attribute:
						// attributes have an OwnerElement, not a ParentNode; also they have
						// to be matched by name, not found by position
						XmlAttribute attribute = (XmlAttribute)node;
						if (attribute.OwnerElement == null) return null;
						sb.Insert(0, string.Concat("/@", node.Name));
						node = attribute.OwnerElement;
						break;
					case XmlNodeType.Element:
						// the only node with no parent is the root node
						index = node.GetIndex(XmlIndexMatchType.Type | XmlIndexMatchType.Name);
						sb.Insert(0, index < 1 ? string.Concat("/", node.Name) : $"/{node.Name}[{index}]");
						node = node.ParentNode;
						break;
					case XmlNodeType.Text:
					case XmlNodeType.SignificantWhitespace:
					case XmlNodeType.CDATA:
						index = node.GetIndex(XmlIndexMatchType.None);
						sb.Insert(0, index < 1 ? "/text()" : $"/node()[{index}]");
						node = node.ParentNode;
						break;
					case XmlNodeType.Comment:
						index = node.GetIndex(XmlIndexMatchType.Type);
						sb.Insert(0, index < 1 ? "/comment()" : $"/comment()[{index}]");
						node = node.ParentNode;
						break;
					case XmlNodeType.EndElement:
						node = node.ParentNode;
						break;
					case XmlNodeType.Document:
						sb.Insert(0, '/');
						node = null;
						break;
					default:
						node = null;
						break;
				}
			}

			return sb.Length == 0 ? null : sb.ToString();
		}

		public static bool IsElement([NotNull] this XmlNode thisValue, string name) { return thisValue.NodeType == XmlNodeType.Element && thisValue.Name.IsSame(name); }

		public static bool IsElement([NotNull] this XmlNode thisValue, string localName, string namespaceURI)
		{
			return thisValue.NodeType == XmlNodeType.Element && thisValue.Name.IsSame(localName) && thisValue.NamespaceURI.IsSame(namespaceURI);
		}
	}
}