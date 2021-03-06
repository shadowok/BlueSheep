﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BlueSheep.Engine.Constants;
using BlueSheep.Interface.Text;

namespace BlueSheep.Interface.UCs
{
    public partial class GestItemsUC : UserControl
    {
        #region Constructors
        public GestItemsUC(AccountUC Account)
        {
            InitializeComponent();
            account = Account;
            LVGestItems.Columns.Add("Nom", 200, HorizontalAlignment.Center);
            LVGestItems.Columns.Add("Action", 200, HorizontalAlignment.Center);
            this.ItemTxtBox.KeyUp += (s, e) =>
            {
                IntelliSense.AutoCompleteTextBox(ItemTxtBox, this.litPopup, IntelliSense.ItemsList, e);
            };
            AutoDeletionTimer = new System.Timers.Timer(Convert.ToDouble(NUDAutoDeletion.Value * 1000));
            AutoDeletionTimer.Elapsed += new System.Timers.ElapsedEventHandler(PerformAutoDeletion);
            if (AutoDeletionBox.Checked)
                AutoDeletionTimer.Start();
        }
        #endregion

        #region Fields
        AccountUC account;
        System.Timers.Timer AutoDeletionTimer;
        #endregion

        #region Public Methods
        public List<int> GetItemsToGetFromBank()
        {
            List<int> items = new List<int>();
            if (LVGestItems.Items.Count <= 0)
                return items;
            else
            {
                foreach (ListViewItem item in LVGestItems.Items)
                {
                    if (item.SubItems[1].Text == "Retirer de la banque")
                        items.Add(account.Inventory.GetItemFromName(item.SubItems[0].Text).UID);
                }
                return items;
            }
        }

        public List<int> GetItemsToTransfer()
        {
            List<int> stayingItems = GetItemsNoBank();
            List<int> items = new List<int>();
            foreach (Core.Inventory.Item i in account.Inventory.Items)
            {
                if (!stayingItems.Contains(i.UID))
                    items.Add(i.UID);
            }
            return items;
        }
        #endregion

        #region PrivateMethods
        private void PerformAutoDeletion(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (account.state == BlueSheep.Engine.Enums.Status.Fighting)
            {
                account.Log(new ErrorTextInformation("La suppression automatique ne peut être effectuée en combat. Suppression automatique annulée. Mais elle reviendra muéhhééhhé"), 2);
                return;
            }
            if (LVGestItems.Items.Count <= 0)
                return;
            else
            {
                foreach (ListViewItem item in LVGestItems.Items)
                {
                    if (item.SubItems[1].Text == "Suppression automatique")
                        account.Inventory.DeleteItem(account.Inventory.GetItemFromName(item.SubItems[0].Text).UID, account.Inventory.GetItemFromName(item.SubItems[0].Text).Quantity);
                }
            }
        }

        private List<int> GetItemsNoBank()
        {
            List<int> items = new List<int>();
            if (LVGestItems.Items.Count <= 0)
                return items;
            else
            {
                foreach (ListViewItem item in LVGestItems.Items)
                {
                    if (item.SubItems[1].Text == "Ne pas déposer en banque")
                        items.Add(account.Inventory.GetItemFromName(item.SubItems[0].Text).UID);
                }
                return items;
            }
        }
        #endregion

        #region UI Methods
        private void AddBt_Click(object sender, EventArgs e)
        {
            if (ItemTxtBox.Text.Length > 0 && ActionChoiceCombo.SelectedText.Length > 0)
            {
                ListViewItem item = new ListViewItem(new string[] { ItemTxtBox.Text, ActionChoiceCombo.SelectedText });
                LVGestItems.Items.Add(item);
                ItemTxtBox.Text = "Entrez le nom d'un item...";
            }

        }

        private void NUDAutoDeletion_ValueChanged(object sender, EventArgs e)
        {
            AutoDeletionTimer = new System.Timers.Timer(Convert.ToDouble(NUDAutoDeletion.Value * 1000));
            AutoDeletionTimer.Elapsed += new System.Timers.ElapsedEventHandler(PerformAutoDeletion);
            if (AutoDeletionBox.Checked)
                AutoDeletionTimer.Start();
        }

        private void AutoDeletionBox_CheckedChanged(object sender)
        {
            if (AutoDeletionBox.Checked)
                AutoDeletionTimer.Start();
            else
                AutoDeletionTimer.Stop();
        }

        private void ItemTxtBox_GotFocus(object sender, EventArgs e)
        {
            if (ItemTxtBox.Text == "Entrez le nom d'un item...")
                ItemTxtBox.Text = "";
        }

        private void ItemTxtBox_LostFocus(object sender, EventArgs e)
        {
            if (ItemTxtBox.Text == "")
                ItemTxtBox.Text = "Entrez le nom d'un item...";
        }
        #endregion

    }
}
