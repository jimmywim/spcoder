using Microsoft.SharePoint.Client;

public class PropertyBagValue
{
    public string Key { get; set; }
    public string Value { get; set; }
}

public class GetListPropertyBag : BasePlugin
{
    public GetListPropertyBag()
    {
        this.TargetType = typeof(Microsoft.SharePoint.Client.List);
        this.Name = "Get List Property Bag";
    }

    public override void Execute(Object target)
    {
        List<PropertyBagValue> propertyBag = new List<PropertyBagValue>();
        this.GetPropertyBag((Microsoft.SharePoint.Client.List)target, propertyBag);

        Result = propertyBag;
        ExecuteCallback(propertyBag);
    }

    private void GetPropertyBag(List list, List<PropertyBagValue> propertyBag)
    {
        var ctx = list.Context as ClientContext;
        ctx.Load(list.RootFolder.Properties);
        ctx.ExecuteQuery();

        foreach (var prop in list.RootFolder.Properties.FieldValues)
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
GetListPropertyBag getPropertyBag = new GetListPropertyBag();
getPropertyBag.Callback += DoShowObjectInGrid;
PluginContainer.Register(getPropertyBag);

logger.LogInfo("Registered plugin GetListPropertyBag");