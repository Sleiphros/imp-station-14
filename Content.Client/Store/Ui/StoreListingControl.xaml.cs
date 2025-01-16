using Content.Client.GameTicking.Managers;
using Content.Shared.Store;
using Robust.Client.AutoGenerated;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Client.Store.Ui;

[GenerateTypedNameReferences]
public sealed partial class StoreListingControl : Control
{
    [Dependency] private readonly IPrototypeManager _prototype = default!;
    [Dependency] private readonly IEntityManager _entity = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    private readonly ClientGameTicker _ticker;

    private readonly ListingDataWithCostModifiers _data;

    private readonly bool _hasBalance;
    private readonly string _price;
    private readonly string _discount;
    private readonly string? _extra; //imp addition
    public StoreListingControl(ListingDataWithCostModifiers data, string price, string discount, bool hasBalance, Texture? texture = null, string? extra = null)
    {
        IoCManager.InjectDependencies(this);
        RobustXamlLoader.Load(this);

        _ticker = _entity.System<ClientGameTicker>();

        _data = data;
        _hasBalance = hasBalance;
        _price = price;
        _discount = discount;
        _extra = extra; //imp addition

        StoreItemName.Text = ListingLocalisationHelpers.GetLocalisedNameOrEntityName(_data, _prototype);
        StoreItemDescription.SetMessage(ListingLocalisationHelpers.GetLocalisedDescriptionOrEntityDescription(_data, _prototype));

        UpdateBuyButtonText();
        StoreItemBuyButton.Disabled = !CanBuy();

        StoreItemTexture.Texture = texture;
    }

    private bool CanBuy()
    {
        if (!_data.Buyable)
            return false;

        if (!_hasBalance)
            return false;

        var stationTime = _timing.CurTime.Subtract(_ticker.RoundStartTimeSpan);
        if (_data.RestockTime > stationTime)
            return false;

        return true;
    }

    private void UpdateBuyButtonText()
    {
        var stationTime = _timing.CurTime.Subtract(_ticker.RoundStartTimeSpan);
        if (_data.RestockTime > stationTime)
        {
            var timeLeftToBuy = stationTime - _data.RestockTime;
            StoreItemBuyButton.Text = timeLeftToBuy.Duration().ToString(@"mm\:ss");
        }
        else if (!_data.Buyable)
        {
            StoreItemBuyButton.Text = "Unavailable";
        }
        else
        {
            DiscountSubText.Text = _discount;
            StoreItemBuyButton.Text = _price;
        }

        //imp addition
        if(_extra != null)
        {
            DiscountSubText.Text = _extra;
        }
    }

    private void UpdateName()
    {
        var name = ListingLocalisationHelpers.GetLocalisedNameOrEntityName(_data, _prototype);

        var stationTime = _timing.CurTime.Subtract(_ticker.RoundStartTimeSpan);
        if (_data.RestockTime > stationTime)
        {
            name += Loc.GetString("store-ui-button-out-of-stock");
        }

        StoreItemName.Text = name;
    }

    protected override void FrameUpdate(FrameEventArgs args)
    {
        base.FrameUpdate(args);

        UpdateBuyButtonText();
        UpdateName();
        StoreItemBuyButton.Disabled = !CanBuy();
    }
}
