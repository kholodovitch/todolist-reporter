using System;
using System.Collections.Generic;
using System.Xml;

namespace nnDev.Components.Common
{
    public static class XmlExtensions
	{
	    public static IEnumerable<XmlNode> GetParents(this XmlNode node, Func<XmlNode, bool> breakFunc)
		{
			if (breakFunc(node))
				yield break;

			if (node.ParentNode != null)
				foreach (var parents in GetParents(node.ParentNode, breakFunc))
					yield return parents;
			yield return node;
		}
    }
}