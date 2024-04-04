﻿using System.Collections.ObjectModel;
using AppListaComprasMaui.Models;


namespace AppListaComprasMaui
{
    public partial class MainPage : ContentPage
    {
        
        ObservableCollection<Produto> Lista_produtos = new ObservableCollection<Produto>();
        public MainPage()
        {
            InitializeComponent();
            lst_produtos.ItemsSource = Lista_produtos;
        }

        

        private void ToolbarItem_Clicked_Somar(object sender, EventArgs e)
        {
            double soma = Lista_produtos.Sum(i => (i.Preco * i.Quantidade));
            string msg = $"O total é{soma:C}";
            DisplayAlert("Somatória", msg, "Fechar");
        }

        protected async override void OnAppearing()
        {
            if (Lista_produtos.Count == 0)
            {

                List<Produto> tmp = await App.Db.GetAll();
                foreach (Produto p in tmp)
                {
                    Lista_produtos.Add(p);
                }
            }
        }

        private async void ToolbarItem_Clicked_Add(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Views.NovoProduto());
        }

        private async void txt_search_TextChanged(object sender, TextChangedEventArgs e)
        {
            string q = e.NewTextValue;
            Lista_produtos.Clear();

            List<Produto> tmp = await App.Db.Search(q);
            foreach(Produto p in tmp)
            {
                Lista_produtos.Add(p);
            }
        }

        private void lst_produtos_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            Produto? p = e.SelectedItem as Produto;
            Navigation.PushAsync(new Views.EditarProduto
            {
                BindingContext = p
            });
        }

        private async void MenuItem_Clicked_Remover(object sender, EventArgs e)
        {
            try
            {
                MenuItem selecionando = (MenuItem)sender;

                Produto p = selecionando.BindingContext as Produto;

                bool confirm = await DisplayAlert(
                    "Tem certeza?", "Remover Produto?",
                    "Sim", "Cancelar");

                if (confirm)
                {
                    await App.Db.Delete(p.Id);
                    await DisplayAlert("Sucesso",
                        "Produto Removido", "OK");
                    Lista_produtos.Remove(p);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ops", ex.Message, "OK");
            }
        }
    }

}
