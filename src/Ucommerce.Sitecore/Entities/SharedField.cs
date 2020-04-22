using System;
using Ucommerce.EntitiesV2;

namespace Ucommerce.Sitecore.Entities
{
	public class SharedField : IEntity
	{
		public virtual int Id { get; set; }

		public virtual Guid ItemId { get; set; }

		public virtual Guid FieldId { get; set; }

		public virtual string FieldValue { get; set; }
	}
}
