using System;
using Sitecore.Commerce.Entities.Carts;

namespace UCommerce.Sitecore.CommerceConnect.Entities.Carts
{
	/// <summary>
	/// Entitiy holding payment info plus amount.
	/// </summary>
	/// <remarks>
	/// The two important properties are <see cref="PaymentInfo.PaymentMethodID"/> and the <see cref="Amount"/>.
	/// </remarks>
	[Serializable]
	public class PaymentInfoWithAmount : PaymentInfo
	{
		/// <summary>
		/// Holds the amount this payment info covers.
		/// </summary>
		public decimal Amount { get; set; }
	}
}
