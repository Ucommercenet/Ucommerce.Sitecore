using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCommerce.Sitecore.Documents
{
	/// <summary>
	/// The class representing a Sitecore item.
	/// </summary>
	public class UcItem
	{
		public Guid Id { get; set; }

		public string Name { get; set; }

		public Guid TemplateId { get; set; }

		public Guid BranchId { get; set; }

		public IList<UcVersionUri> Versions { get; set; }

		public Guid ParentId { get; set; }
	}

	public class UcVersionUri
	{
		public string Language { get; set; }

		public int Version { get; set; }

		public IList<KeyValuePair<Guid, string>> FieldList { get; set; }
	}
}
