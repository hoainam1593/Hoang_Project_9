
public class IAPImplementation_editor : IIAPImplementation
{
    public IAPImplementation_editor()
    {
        IAPController.instance.CurrencyCode = "VND";
    }
    
    public string GetPrice(string productId)
    {
        return "0.0 USD";
    }
    
    public void Purchase(string productId)
    {
        IAPController.instance.listener.OnRewardPlayer(productId);
    }

    public void RestorePurchase()
    {
    }
}