﻿using Blazored.LocalStorage;
using TroubleTrack.Models;

namespace TroubleTrack.Services
{
    public class UserPreferencesService
    {
        private readonly ILocalStorageService _localStorage;
        private const string Key = "userPreferences";

        public UserPreferencesService(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public async Task SaveUserPreferences(UserPreferences userPreferences)
        {
            await _localStorage.SetItemAsync(Key, userPreferences);
        }

        public async Task<UserPreferences> LoadUserPreferences()
        {
            return await _localStorage.GetItemAsync<UserPreferences>(Key);
        }
    }
}
