using System;
using UCommerce.EntitiesV2;

namespace UCommerce.Sitecore.Entities
{
	public class SharedField : IEntity
	{
		public virtual int Id { get; set; }

		public virtual Guid ItemId { get; set; }

		public virtual Guid FieldId { get; set; }

		public virtual string FieldValue { get; set; }
	}
}
