
public interface IIAPImplementation
{
	string GetPrice(string productId);
	void Purchase(string productId);
	void RestorePurchase();
}