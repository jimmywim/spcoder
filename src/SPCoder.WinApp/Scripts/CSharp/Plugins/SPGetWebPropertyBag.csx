using Microsoft.SharePoint.Client;

public class PropertyBagValue
{
    public string Key { get; set; }
    public string Value { get; set; }
}

public class GetWebPropertyBag : BasePlugin
{
    public GetWebPropertyBag()
    {
        this.TargetType = typeof(Microsoft.SharePoint.Client.Web);
        this.Name = "Get Web Property Bag";
    }

    public override void Execute(Object target)
    {
        List<PropertyBagValue> propertyBag = new List<PropertyBagValue>();
        this.GetPropertyBag((Microsoft.SharePoint.Client.Web)target, propertyBag);

        Result = propertyBag;
        ExecuteCallback(propertyBag);
    }

    private void GetPropertyBag(Web web, List<PropertyBagValue> propertyBag)
    {
        var ctx = web.Context as ClientContext;
        ctx.Load(web.AllProperties);
        ctx.ExecuteQuery();

        foreach (var prop in web.AllProperties.FieldValues)
        {
            propertyBag.Add(new PropertyBagValue
            {
                Key = prop.Key,
                Value = prop.Value.ToString()
            });
        }

        propertyBag = propertyBag.OrderBy(p => p.Key).ToList();
    }
}

//registration code
GetWebPropertyBag getPropertyBag = new GetWebPropertyBag();
getPropertyBag.Callback += DoShowObjectInGrid;
PluginContainer.Register(getPropertyBag);

logger.LogInfo("Registered plugin GetWebPropertyBag");