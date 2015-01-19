# EnumAttributeHelper
Custom attribute and helpers to make enums in Entity Framework Code First models easier to use. 

Using enums with CF models is a very powerful way to create a strongly typed range of values for a field. The only drawback is that setting up corresponding string values for display requires a lot of boilerplate code. For every enum, you end up writing two methods with switch statements to get data from string <-> enum. 

To reduce this, I wrote the following helper classes for enums that works with custom attributes.

First is the BaseEnumAttribute, with a single Value field, followed by the EnumDisplay attribute that extends from it.

	public class EnumDisplay : BaseEnumAttribute
    {
        public EnumDisplay(string value)
            : base(value)
        {
        }
    }

    public abstract class BaseEnumAttribute : Attribute
    {
        public string Value { get; private set; }

        protected BaseEnumAttribute(string value)
        {
            Value = value;
        }
    }

The EnumHelper class has 2 generic methods SelectEnum and Get.

`SelectEnum<E, A>(string value)` simply gets the enum based on the value of an attribute.
For example, this will return CategoryTypes.Arts: 

  `var result = EnumHelper.SelectEnum<CategoryTypes, EnumDisplay>("Arts");`

`Get<T>(this Enum enumVal)` simply gets the value of an attribute for a enum.
For example, this will return "Arts":

  `string podcastCategory = Podcast.CategoryTypes.Arts.Get<EnumDisplay>();`

 EnumHelper.cs

	public static class EnumHelper
    {
        /// <summary>
        /// Gets first enum with desired value of selected BaseEnumAttribute
        /// </summary>
        /// <typeparam name="E">The enum you searching</typeparam>
        /// <typeparam name="A">The BaseEnumAttribute you are searching by</typeparam>
        /// <param name="value"></param>
        /// <returns>Enum, null if not found</returns>
        public static Enum SelectEnum<E,A>(string value)
            where E : struct, IConvertible, IComparable, IFormattable
            where A : BaseEnumAttribute
        {
            return Enum.GetValues(typeof(E)).Cast<Enum>().FirstOrDefault(e => e.Get<A>() == value);
        }

        /// <summary>
        /// Gets the value of an EnumAttribute
        /// </summary>
        /// <typeparam name="T">The type of the EnumAttribute you want the value of</typeparam>
        /// <param name="enumVal">The enum value</param>
        /// <returns>String value of EnumAttribute</returns>
        public static string Get<T>(this Enum enumVal) where T : BaseEnumAttribute
        {
            var type = enumVal.GetType();
            var member = type.GetMember(enumVal.ToString()).FirstOrDefault();
            if (member != null)
            {
                var attributes = member.GetCustomAttributes(typeof(T), false);

                if (attributes.Any())
                {
                    var attribute = (T)attributes[0];
                    return attribute.Value;
                }
            }
            return string.Empty;
        }
    }

**Reccommeded usage in the model:**

`EnumHelper.SelectEnum<CategoryTypes, EnumDisplay>("Arts");`is still to wordy for me, so with each enum I create a simple corresponding method for SelectEnum<E, A>

	public static CategoryTypes SelectCategoryType(string enumDisplay)
    {
        var result = EnumHelper.SelectEnum<CategoryTypes, EnumDisplay>(enumDisplay);
        return (result == null ? CategoryTypes.NotSet : (CategoryTypes)result);
    }

This means rather than this: 

  `string podcastCategory = EnumHelper.SelectEnum<CategoryTypes, EnumDisplay>("Arts");`
  
We can use this: 

  `var category = Podcast.SelectCategoryType("Arts");`

No generics, just a simple method.

Full example model:

	public class Podcast
    {
        public CategoryTypes Category { get; set; }

        public enum CategoryTypes
        {
            [EnumDisplay(CategoryTypesConsts.NotSet)]
            NotSet = 0,

            [EnumDisplay(CategoryTypesConsts.Arts)]
            Arts = 1,

            [EnumDisplay(CategoryTypesConsts.Business)]
            Business = 2,

            [EnumDisplay(CategoryTypesConsts.Comedy)]
            Comedy = 3,

            [EnumDisplay(CategoryTypesConsts.GovernmentOrganizations)]
            GovernmentOrganizations = 4,
        }

        public static CategoryTypes SelectCategoryType(string enumDisplay)
        {
            var result = EnumHelper.SelectEnum<CategoryTypes, EnumDisplay>(enumDisplay);
            return (result == null ? CategoryTypes.NotSet : (CategoryTypes)result);
        }

        public static class CategoryTypesConsts
        {
            public const string NotSet = "Not Set";
            public const string Arts = "Arts";
            public const string Business = "Business";
            public const string Education = "Education";
            public const string Comedy = "Comedy";
            public const string GamesHobbies = "Games & Hobbies";
            public const string GovernmentOrganizations = "Government & Organizations";
        }
    }
