namespace DataCollector.Utilities
{
    public static class ApiRoutes
    {
        public const string Root = "api";
        public const string Version = "v1";
        public const string Base = Root + "/" + Version;


        public static class Identity
        {
            public const string Login = Base + "/users/auth/login";
            public const string Delete = Base + "/users/{userId}";
            public const string Update = Base + "/users/{userId}";
            public const string Get = Base + "/users/{userId}";
            public const string GetAll = Base + "/users/";
            public const string Create = Base + "/users/";
            public const string Register = Base + "/users/auth/register";
            public const string ResetMyPassword = Base + "/users/password";
            public const string GetMyProfile = Base + "/users/profile";
            public const string UpdateMyProfile = Base + "/users/profile";
            public const string ResetPassword = Base + "/users/{userId}/password";
        }


        public static class Manage
        {
            public const string EditProfile = Base + "/users/profile";
            public const string GetProfile = Base + "/users/profile";
        }


        public static class Category
        {
            public const string GetAllCategories = Base + "/categories";
            public const string GetCategoryById = Base + "/categories/{id}";
            public const string CreateCategory = Base + "/categories";
            public const string UpdateCategory = Base + "/categories/{id}";
            public const string DeleteCategory = Base + "/categories/{id}";
        }

        public static class Store
        {
            public const string GetAllStores= Base + "/stores";
            public const string ExportToExcel = Base + "/stores/export";
            public const string GetCreatedStoresCountByUser = Base + "/stores/user/count";
            public const string GetStoreById = Base + "/stores/{id}";
            public const string CreateStore = Base + "/stores";
            public const string AddCollectorNote = Base + "/stores/{id}/collectorNote";
            public const string DeleteStore = Base + "/stores/{id}";
            public const string UpdateStore = Base + "/stores/{id}";
        }

    }
}
