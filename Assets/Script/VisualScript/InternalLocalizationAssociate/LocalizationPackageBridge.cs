using UnityEditor.Search;
using UnityEditor.Localization;
using UnityEditor.Localization.Search;
using UnityEngine.Localization.Tables;

namespace UnityEditor.Localization.UI{

    // class LocalizationPackageBridge : LocalizedReferencePropertyDrawer<StringTableCollection>{
    //     public void ShowPicker(){
    //         var provider = new StringTableSearchProvider();
    //         var context = UnityEditor.Search.SearchService.CreateContext(provider, provider.filterId);
    //         var picker = new LocalizedReferencePicker<StringTableCollection>(context, "string table entry", null, this);
    //         picker.Show();
    //     }
    // }

    public class LocalizationPackageBridge {
        public void ShowPicker(){
            var provider = new StringTableSearchProvider();
            var context = UnityEditor.Search.SearchService.CreateContext(provider, provider.filterId);
            UnityEditor.Search.SearchService.ShowPicker(context, Select, Track, null, null, "Title");
        }

        void Select(SearchItem item, bool cancelled)
        {
        }

        void Track(SearchItem item)
        {
        }

        void SetItem(LocalizationTableCollection collection, SharedTableData.SharedTableEntry entry, bool collapse)
        {
        }
    }
}