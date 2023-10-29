﻿using BHSystem.Web.ViewModels;
using BHSytem.Models.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace BHSystem.Web.Features.Admin
{
    public partial class UserPage
    {
        public List<UserModel>? ListUser { get; set; }
        public IEnumerable<UserModel>? SelectedUsers { get; set; } = new List<UserModel>();
        public bool IsInitialDataLoadComplete { get; set; } = true;
        public UserModel UserUpdate { get; set; } = new UserModel();

        public bool IsShowDialog { get; set; }
        public EditContext? _EditContext { get; set; }

        protected override Task OnInitializedAsync()
        {
            ListUser = new List<UserModel>();
            ListUser.Add(new UserModel() { UserId = 1, FullName = "Nguyễn Tấn Hải" });
            ListUser.Add(new UserModel() { UserId = 2, FullName = "Nguyễn Tấn Hải" });
            ListUser.Add(new UserModel() { UserId = 3, FullName = "Nguyễn Tấn Hải" });
            ListUser.Add(new UserModel() { UserId = 4, FullName = "Nguyễn Tấn Hải" });
            ListUser.Add(new UserModel() { UserId = 5, FullName = "Nguyễn Tấn Hải" });
            ListUser.Add(new UserModel() { UserId = 6, FullName = "Nguyễn Tấn Hải" });
            return base.OnInitializedAsync();
        }

        protected void OnOpenDialogHandler()
        {
            IsShowDialog = true;
            _EditContext = new EditContext(UserUpdate);
        }


        protected async void SaveDataHandler(EnumType pEnum = EnumType.SaveAndClose)
        {
            string sAction = nameof(EnumType.Add);
            var checkData = _EditContext!.Validate();
            if (!checkData) return;
        }

    }
}
