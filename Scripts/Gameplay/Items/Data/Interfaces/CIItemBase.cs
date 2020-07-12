
namespace CDK {
	public interface CIItemBase {

		CItemBaseScriptableObject GetScriptableObject();
		int Count { get; }
		int Add(int quantity);
		int Remove(int quantity);

	}
}
