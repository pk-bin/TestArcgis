using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace TestArcgis.shellpage
{
    public partial class PlaceSearchPage : ContentPage
    {
        public ObservableCollection<Model.VworldPlacePoi.PoiItem> Items = new ObservableCollection<Model.VworldPlacePoi.PoiItem>();
        private Action<Model.VworldPlacePoi.PoiItem> mSelectItem;
        private Model.VworldPlacePoi.Page mSearchPage;
        private string mAddress;
        private Xamarin.Essentials.GeolocationRequest mRequest = null;

        public PlaceSearchPage(string pAddress, Action<Model.VworldPlacePoi.PoiItem> pReturnSelectItemCallback)
        {
            InitializeComponent();

            mSelectItem = pReturnSelectItemCallback;
            MyListView.ItemsSource = Items;
            mAddress = pAddress;

            //GetPlaceSearchListData(0);
        }

        private void GetPlaceSearchListData(int pIndex)
        {
            xBusyIndicator.IsRunning = true;

            if (mSearchPage == null)
            {
                var result = Common.LocationRestAPI.GeVWorldAddressList(mAddress, 1);
                mSearchPage = result.response.page;

                xPageTextLabel.Text = mSearchPage.current;

                Items.Clear();
                foreach (var _items in result.response.result.items)
                {
                    Items.Add(_items);
                }

                if (Convert.ToInt32(mSearchPage.current) == Convert.ToInt32(mSearchPage.total))
                {
                    xBeforeButton.IsEnabled = false;
                    xNextButton.IsEnabled = false;
                }
                else
                {
                    xBeforeButton.IsEnabled = false;
                    xNextButton.IsEnabled = true;
                }
            }
            else
            {
                if (pIndex < 0)
                {
                    var result = Common.LocationRestAPI.GeVWorldAddressList(mAddress, Convert.ToInt32(mSearchPage.current) - 1);
                    mSearchPage = result.response.page;
                    xPageTextLabel.Text = mSearchPage.current;
                    Items.Clear();
                    foreach (var _items in result.response.result.items)
                    {
                        Items.Add(_items);
                    }

                    if (Convert.ToInt32(mSearchPage.current) == 1)
                    {
                        xBeforeButton.IsEnabled = false;
                    }
                    else
                    {
                        xBeforeButton.IsEnabled = true;
                    }
                    xNextButton.IsEnabled = true;
                }
                else if (pIndex > 0)
                {
                    var result = Common.LocationRestAPI.GeVWorldAddressList(mAddress, Convert.ToInt32(mSearchPage.current) + 1);
                    mSearchPage = result.response.page;
                    xPageTextLabel.Text = mSearchPage.current;
                    Items.Clear();
                    foreach (var _items in result.response.result.items)
                    {
                        Items.Add(_items);
                    }

                    if (Convert.ToInt32(mSearchPage.current) == Convert.ToInt32(mSearchPage.total))
                    {
                        xNextButton.IsEnabled = false;
                    }
                    else
                    {
                        xNextButton.IsEnabled = true;
                    }
                    xBeforeButton.IsEnabled = true;
                }
                else
                {
                    var result = Common.LocationRestAPI.GeVWorldAddressList(mAddress, 1);
                    mSearchPage = result.response.page;

                    xPageTextLabel.Text = mSearchPage.current;

                    Items.Clear();
                    foreach (var _items in result.response.result.items)
                    {
                        Items.Add(_items);
                    }

                    if (Convert.ToInt32(mSearchPage.current) == Convert.ToInt32(mSearchPage.total))
                    {
                        xBeforeButton.IsEnabled = false;
                        xNextButton.IsEnabled = false;
                    }
                    else
                    {
                        xBeforeButton.IsEnabled = false;
                        xNextButton.IsEnabled = true;
                    }
                }
            }

            xBusyIndicator.IsRunning = false;
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            Model.VworldPlacePoi.PoiItem target = e.Item as Model.VworldPlacePoi.PoiItem;

            mSelectItem(target);
            ((ListView)sender).SelectedItem = null;

            await Navigation.PopAsync();
        }

        private async void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            await Navigation.PopAsync();
        }


        private void xBeforeButton_Clicked(object sender, EventArgs e)
        {
            GetPlaceSearchListData(-1);
        }

        private void xNextButton_Clicked(object sender, EventArgs e)
        {
            GetPlaceSearchListData(1);
        }
    }
}
