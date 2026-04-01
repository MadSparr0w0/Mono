using UnityEngine;

public enum SpaceType
{
    Start,
    Street,
    Utility,
    WaterWorks,
    ElectricCompany,
    Railroad,
    Chance,
    CommunityChest,
    Tax,
    Jail,
    GoToJail,
    FreeParking,
    Special
}

public class BoardSpace : MonoBehaviour
{
    [Header("Base Info")]
    [SerializeField] private int index = 0;
    [SerializeField] private string spaceName = "New Space";
    [SerializeField] private SpaceType spaceType = SpaceType.Special;
    [SerializeField] private Transform pawnAnchor;

    [Header("Buy Settings")]
    [SerializeField] private bool purchasable = false;
    [SerializeField] private int purchasePrice = 0;

    [Header("Street")]
    [SerializeField] private int streetBaseRent = 10;
    [SerializeField] private string streetColorGroup = "";

    [Header("Utility")]
    [SerializeField] private int utilityDiceMultiplier = 4;

    [Header("Water Works")]
    [SerializeField] private int waterWorksDiceMultiplier = 4;

    [Header("Electric Company")]
    [SerializeField] private int electricCompanyDiceMultiplier = 4;

    [Header("Railroad")]
    [SerializeField] private int railroadBaseRent = 25;

    [Header("Tax / Special")]
    [SerializeField] private int taxAmount = 0;

    [TextArea(2, 5)]
    [SerializeField] private string note = "";

    public int Index => index;
    public string SpaceName => spaceName;
    public SpaceType Type => spaceType;
    public SpaceType SpaceType => spaceType;
    public Transform PawnAnchor => pawnAnchor;

    public bool Purchasable => purchasable;
    public int PurchasePrice => purchasePrice;
    public int StreetBaseRent => streetBaseRent;
    public string StreetColorGroup => streetColorGroup;
    public int UtilityDiceMultiplier => utilityDiceMultiplier;
    public int WaterWorksDiceMultiplier => waterWorksDiceMultiplier;
    public int ElectricCompanyDiceMultiplier => electricCompanyDiceMultiplier;
    public int RailroadBaseRent => railroadBaseRent;
    public int TaxAmount => taxAmount;
    public string Note => note;

    public Vector3 GetPawnPosition()
    {
        return pawnAnchor != null ? pawnAnchor.position : transform.position;
    }

    public Vector3 GetWorldPoint()
    {
        return GetPawnPosition();
    }

    private void OnValidate()
    {
        if (string.IsNullOrWhiteSpace(spaceName))
            spaceName = gameObject.name;

        switch (spaceType)
        {
            case SpaceType.Street:
                purchasable = true;
                if (purchasePrice <= 0) purchasePrice = 100;
                if (streetBaseRent <= 0) streetBaseRent = 10;
                break;

            case SpaceType.Utility:
                purchasable = true;
                if (purchasePrice <= 0) purchasePrice = 150;
                if (utilityDiceMultiplier <= 0) utilityDiceMultiplier = 4;
                break;

            case SpaceType.WaterWorks:
                purchasable = true;
                if (purchasePrice <= 0) purchasePrice = 150;
                if (waterWorksDiceMultiplier <= 0) waterWorksDiceMultiplier = 4;
                break;

            case SpaceType.ElectricCompany:
                purchasable = true;
                if (purchasePrice <= 0) purchasePrice = 150;
                if (electricCompanyDiceMultiplier <= 0) electricCompanyDiceMultiplier = 4;
                break;

            case SpaceType.Railroad:
                purchasable = true;
                if (purchasePrice <= 0) purchasePrice = 200;
                if (railroadBaseRent <= 0) railroadBaseRent = 25;
                break;

            case SpaceType.Tax:
                purchasable = false;
                if (taxAmount <= 0) taxAmount = 100;
                break;

            default:
                if (spaceType != SpaceType.Street &&
                    spaceType != SpaceType.Utility &&
                    spaceType != SpaceType.WaterWorks &&
                    spaceType != SpaceType.ElectricCompany &&
                    spaceType != SpaceType.Railroad)
                {
                    purchasable = false;
                }
                break;
        }
    }

    public int CalculateRent(int currentDiceTotal = 0, int ownedRailroads = 1)
    {
        switch (spaceType)
        {
            case SpaceType.Street:
                return streetBaseRent;
            case SpaceType.Utility:
                return currentDiceTotal * utilityDiceMultiplier;
            case SpaceType.WaterWorks:
                return currentDiceTotal * waterWorksDiceMultiplier;
            case SpaceType.ElectricCompany:
                return currentDiceTotal * electricCompanyDiceMultiplier;
            case SpaceType.Railroad:
                switch (Mathf.Clamp(ownedRailroads, 1, 4))
                {
                    case 1: return railroadBaseRent;
                    case 2: return railroadBaseRent * 2;
                    case 3: return railroadBaseRent * 4;
                    case 4: return railroadBaseRent * 8;
                    default: return railroadBaseRent;
                }
            default:
                return 0;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (pawnAnchor == null) return;
        Gizmos.DrawWireSphere(pawnAnchor.position, 0.12f);
        Gizmos.DrawLine(transform.position, pawnAnchor.position);
    }
#endif
}
