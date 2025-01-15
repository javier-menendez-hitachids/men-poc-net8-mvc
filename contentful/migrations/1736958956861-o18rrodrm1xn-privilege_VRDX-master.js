function migrationFunction(migration, context) {
    const villageProperties = migration.createContentType("villageProperties");
    villageProperties
        .displayField("id")
        .name("Village Properties")
        .description("")

    const villagePropertiesId = villageProperties.createField("id");
    villagePropertiesId
        .name("Id")
        .type("Symbol")
        .localized(false)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const villagePropertiesVillageId = villageProperties.createField("villageId");
    villagePropertiesVillageId
        .name("Village Id")
        .type("Symbol")
        .localized(false)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const villagePropertiesFirstName = villageProperties.createField("firstName");
    villagePropertiesFirstName
        .name("First Name")
        .type("Link")
        .localized(true)
        .required(false)
        .validations([{ "linkContentType": ["formField"] }])
        .disabled(false)
        .omitted(false)
        .linkType("Entry")

    const villagePropertiesLastName = villageProperties.createField("lastName");
    villagePropertiesLastName
        .name("Last Name")
        .type("Link")
        .localized(true)
        .required(false)
        .validations([{ "linkContentType": ["formField"] }])
        .disabled(false)
        .omitted(false)
        .linkType("Entry")

    const villagePropertiesBirthdate = villageProperties.createField("birthdate");
    villagePropertiesBirthdate
        .name("Birthdate")
        .type("Link")
        .localized(true)
        .required(false)
        .validations([{ "linkContentType": ["birthdate"] }])
        .disabled(false)
        .omitted(false)
        .linkType("Entry")

    const villagePropertiesMobile = villageProperties.createField("mobile");
    villagePropertiesMobile
        .name("Mobile")
        .type("Link")
        .localized(true)
        .required(false)
        .validations([{ "linkContentType": ["formField"] }])
        .disabled(false)
        .omitted(false)
        .linkType("Entry")

    const villagePropertiesGender = villageProperties.createField("gender");
    villagePropertiesGender
        .name("Gender")
        .type("Link")
        .localized(true)
        .required(false)
        .validations([{ "linkContentType": ["formField"] }])
        .disabled(false)
        .omitted(false)
        .linkType("Entry")

    const villagePropertiesCountryOfResidence = villageProperties.createField("countryOfResidence");
    villagePropertiesCountryOfResidence
        .name("Country of Residence")
        .type("Link")
        .localized(true)
        .required(false)
        .validations([{ "linkContentType": ["formField"] }])
        .disabled(false)
        .omitted(false)
        .linkType("Entry")

    const villagePropertiesCounty = villageProperties.createField("county");
    villagePropertiesCounty
        .name("County")
        .type("Link")
        .localized(true)
        .required(false)
        .validations([{ "linkContentType": ["formField"] }])
        .disabled(false)
        .omitted(false)
        .linkType("Entry")

    const villagePropertiesPostcode = villageProperties.createField("postcode");
    villagePropertiesPostcode
        .name("Postcode")
        .type("Link")
        .localized(true)
        .required(false)
        .validations([{ "linkContentType": ["formField"] }])
        .disabled(false)
        .omitted(false)
        .linkType("Entry")
    villageProperties.changeFieldControl("id", "builtin", "singleLine")
    villageProperties.changeFieldControl("villageId", "builtin", "singleLine")
    villageProperties.changeFieldControl("firstName", "builtin", "entryLinkEditor")
    villageProperties.changeFieldControl("lastName", "builtin", "entryLinkEditor")
    villageProperties.changeFieldControl("birthdate", "builtin", "entryLinkEditor")
    villageProperties.changeFieldControl("mobile", "builtin", "entryLinkEditor")
    villageProperties.changeFieldControl("gender", "builtin", "entryLinkEditor")
    villageProperties.changeFieldControl("countryOfResidence", "builtin", "entryLinkEditor")
    villageProperties.changeFieldControl("county", "builtin", "entryLinkEditor")
    villageProperties.changeFieldControl("postcode", "builtin", "entryLinkEditor")

    const globalProperties = migration.createContentType("globalProperties");
    globalProperties
        .displayField("id")
        .name("Global Properties")
        .description("")

    const globalPropertiesId = globalProperties.createField("id");
    globalPropertiesId
        .name("Id")
        .type("Symbol")
        .localized(false)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const globalPropertiesLevelTiers = globalProperties.createField("levelTiers");
    globalPropertiesLevelTiers
        .name("Level Tiers")
        .type("Array")
        .localized(false)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)
        .items({ "type": "Link", "validations": [{ "linkContentType": ["levelTier"] }], "linkType": "Entry" })

    const globalPropertiesLevelLabel = globalProperties.createField("levelLabel");
    globalPropertiesLevelLabel
        .name("Level Label ")
        .type("Symbol")
        .localized(true)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const globalPropertiesDasboardRefreshRetryInterval = globalProperties.createField("dasboardRefreshRetryInterval");
    globalPropertiesDasboardRefreshRetryInterval
        .name("Dasboard Refresh Retry Interval (secs)")
        .type("Integer")
        .localized(false)
        .required(false)
        .validations([{ "range": { "min": 1, "max": 30 }, "message": "Number shoud be between 1 and 30" }])
        .defaultValue({ "en-US": 10 })
        .disabled(false)
        .omitted(false)

    const globalPropertiesDasboardRefreshMaxRetries = globalProperties.createField("dasboardRefreshMaxRetries");
    globalPropertiesDasboardRefreshMaxRetries
        .name("Dasboard Refresh Max Retries")
        .type("Integer")
        .localized(false)
        .required(false)
        .validations([{ "range": { "min": 0, "max": 3 }, "message": "Number should be between 0 and 3" }])
        .defaultValue({ "en-US": 2 })
        .disabled(false)
        .omitted(false)
    globalProperties.changeFieldControl("id", "builtin", "singleLine")
    globalProperties.changeFieldControl("levelTiers", "builtin", "entryLinksEditor")
    globalProperties.changeFieldControl("levelLabel", "builtin", "singleLine")
    globalProperties.changeFieldControl("dasboardRefreshRetryInterval", "builtin", "numberEditor")
    globalProperties.changeFieldControl("dasboardRefreshMaxRetries", "builtin", "numberEditor")

    const resourceSet = migration.createContentType("resourceSet");
    resourceSet
        .displayField("name")
        .name("Resource set")
        .description("")

    const resourceSetName = resourceSet.createField("name");
    resourceSetName
        .name("Name")
        .type("Symbol")
        .localized(false)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const resourceSetResources = resourceSet.createField("resources");
    resourceSetResources
        .name("Resources")
        .type("Array")
        .localized(false)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)
        .items({ "type": "Link", "validations": [{ "linkContentType": ["resource"] }], "linkType": "Entry" })
    resourceSet.changeFieldControl("name", "builtin", "singleLine")
    resourceSet.changeFieldControl("resources", "builtin", "entryLinksEditor")

    const resource = migration.createContentType("resource");
    resource
        .displayField("key")
        .name("Resource")
        .description("To hold resources, properties, etc... with key/value pairs")

    const resourceKey = resource.createField("key");
    resourceKey
        .name("Key")
        .type("Symbol")
        .localized(false)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const resourceValue = resource.createField("value");
    resourceValue
        .name("Value")
        .type("Text")
        .localized(true)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)
    resource.changeFieldControl("key", "builtin", "singleLine")
    resource.changeFieldControl("value", "builtin", "markdown")

    const levelTier = migration.createContentType("levelTier");
    levelTier
        .displayField("name")
        .name("Level Tier")
        .description("To hold level tier colour, text and Title")

    const levelTierName = levelTier.createField("name");
    levelTierName
        .name("Name")
        .type("Symbol")
        .localized(false)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const levelTierTitle = levelTier.createField("title");
    levelTierTitle
        .name("Title")
        .type("Symbol")
        .localized(true)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const levelTierText = levelTier.createField("text");
    levelTierText
        .name("Text")
        .type("Symbol")
        .localized(true)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const levelTierColour = levelTier.createField("colour");
    levelTierColour
        .name("Colour")
        .type("Symbol")
        .localized(false)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)
    levelTier.changeFieldControl("name", "builtin", "singleLine")
    levelTier.changeFieldControl("title", "builtin", "singleLine")
    levelTier.changeFieldControl("text", "builtin", "singleLine")
    levelTier.changeFieldControl("colour", "builtin", "singleLine")

    const standardHeroWidget = migration.createContentType("standardHeroWidget");
    standardHeroWidget
        .displayField("name")
        .name("Standard Hero Widget")
        .description("Standard Hero Widget")

    const standardHeroWidgetName = standardHeroWidget.createField("name");
    standardHeroWidgetName
        .name("Name")
        .type("Symbol")
        .localized(false)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const standardHeroWidgetIWidget = standardHeroWidget.createField("iWidget");
    standardHeroWidgetIWidget
        .name("IWidget")
        .type("Link")
        .localized(false)
        .required(false)
        .validations([{ "linkContentType": ["iWidget"] }])
        .disabled(false)
        .omitted(false)
        .linkType("Entry")

    const standardHeroWidgetHeading = standardHeroWidget.createField("heading");
    standardHeroWidgetHeading
        .name("Heading")
        .type("Symbol")
        .localized(true)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const standardHeroWidgetButtonText = standardHeroWidget.createField("buttonText");
    standardHeroWidgetButtonText
        .name("ButtonText")
        .type("Symbol")
        .localized(true)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const standardHeroWidgetUrl = standardHeroWidget.createField("url");
    standardHeroWidgetUrl
        .name("Url")
        .type("Symbol")
        .localized(false)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const standardHeroWidgetOpenInNewWindow = standardHeroWidget.createField("openInNewWindow");
    standardHeroWidgetOpenInNewWindow
        .name("OpenInNewWindow")
        .type("Boolean")
        .localized(false)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const standardHeroWidgetIsInvertedOverlay = standardHeroWidget.createField("isInvertedOverlay");
    standardHeroWidgetIsInvertedOverlay
        .name("IsInvertedOverlay")
        .type("Boolean")
        .localized(false)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const standardHeroWidgetImageSrc = standardHeroWidget.createField("imageSrc");
    standardHeroWidgetImageSrc
        .name("ImageSrc")
        .type("Symbol")
        .localized(false)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const standardHeroWidgetBgColour = standardHeroWidget.createField("bgColour");
    standardHeroWidgetBgColour
        .name("BgColour")
        .type("Symbol")
        .localized(false)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)
    standardHeroWidget.changeFieldControl("name", "builtin", "singleLine")
    standardHeroWidget.changeFieldControl("iWidget", "builtin", "entryLinkEditor")
    standardHeroWidget.changeFieldControl("heading", "builtin", "singleLine")
    standardHeroWidget.changeFieldControl("buttonText", "builtin", "singleLine")
    standardHeroWidget.changeFieldControl("url", "builtin", "singleLine")
    standardHeroWidget.changeFieldControl("openInNewWindow", "builtin", "boolean")
    standardHeroWidget.changeFieldControl("isInvertedOverlay", "builtin", "boolean")
    standardHeroWidget.changeFieldControl("imageSrc", "builtin", "singleLine")
    standardHeroWidget.changeFieldControl("bgColour", "builtin", "singleLine")

    const iWidget = migration.createContentType("iWidget");
    iWidget
        .displayField("columnSize")
        .name("IWidget")
        .description("Common Widget interface")

    const iWidgetId = iWidget.createField("id");
    iWidgetId
        .name("Id")
        .type("Integer")
        .localized(false)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const iWidgetColumnSize = iWidget.createField("columnSize");
    iWidgetColumnSize
        .name("Column Size")
        .type("Symbol")
        .localized(false)
        .required(false)
        .validations([{ "in": ["OneCol", "TwoCol", "ThreeCol", "Half"] }])
        .disabled(false)
        .omitted(false)

    const iWidgetIsLazy = iWidget.createField("isLazy");
    iWidgetIsLazy
        .name("IsLazy")
        .type("Boolean")
        .localized(false)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const iWidgetIsHiddenIfOddLast = iWidget.createField("isHiddenIfOddLast");
    iWidgetIsHiddenIfOddLast
        .name("IsHiddenIfOddLast")
        .type("Boolean")
        .localized(false)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const iWidgetIgnoreLastOdd = iWidget.createField("ignoreLastOdd");
    iWidgetIgnoreLastOdd
        .name("IgnoreLastOdd")
        .type("Boolean")
        .localized(false)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)
    iWidget.changeFieldControl("id", "builtin", "numberEditor")
    iWidget.changeFieldControl("columnSize", "builtin", "singleLine")
    iWidget.changeFieldControl("isLazy", "builtin", "boolean")
    iWidget.changeFieldControl("isHiddenIfOddLast", "builtin", "boolean")
    iWidget.changeFieldControl("ignoreLastOdd", "builtin", "boolean")

    const notFoundPage = migration.createContentType("notFoundPage");
    notFoundPage
        .displayField("id")
        .name("Not Found Page")
        .description("")

    const notFoundPageId = notFoundPage.createField("id");
    notFoundPageId
        .name("Id")
        .type("Symbol")
        .localized(false)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const notFoundPageVillageId = notFoundPage.createField("villageId");
    notFoundPageVillageId
        .name("Village Id")
        .type("Symbol")
        .localized(false)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const notFoundPageIntroBlock = notFoundPage.createField("introBlock");
    notFoundPageIntroBlock
        .name("Page Name & Body")
        .type("Link")
        .localized(false)
        .required(false)
        .validations([{ "linkContentType": ["richTextBlock"] }])
        .disabled(false)
        .omitted(false)
        .linkType("Entry")

    const notFoundPageWidgetSlotOne = notFoundPage.createField("widgetSlotOne");
    notFoundPageWidgetSlotOne
        .name("Widget Slot One")
        .type("Array")
        .localized(false)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)
        .items({ "type": "Link", "validations": [{ "linkContentType": ["standardHeroWidget"] }], "linkType": "Entry" })

    const notFoundPageWidgetSlotTwo = notFoundPage.createField("widgetSlotTwo");
    notFoundPageWidgetSlotTwo
        .name("Widget Slot Two")
        .type("Array")
        .localized(false)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)
        .items({ "type": "Link", "validations": [{ "linkContentType": ["standardHeroWidget"] }], "linkType": "Entry" })
    notFoundPage.changeFieldControl("id", "builtin", "singleLine")
    notFoundPage.changeFieldControl("villageId", "builtin", "singleLine")
    notFoundPage.changeFieldControl("introBlock", "builtin", "entryLinkEditor")
    notFoundPage.changeFieldControl("widgetSlotOne", "builtin", "entryLinksEditor")
    notFoundPage.changeFieldControl("widgetSlotTwo", "builtin", "entryLinksEditor")

    const forgottenPasswordPage = migration.createContentType("forgottenPasswordPage");
    forgottenPasswordPage
        .displayField("id")
        .name("Forgotten Password Page")
        .description("")

    const forgottenPasswordPageId = forgottenPasswordPage.createField("id");
    forgottenPasswordPageId
        .name("Id")
        .type("Symbol")
        .localized(false)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const forgottenPasswordPageVillageId = forgottenPasswordPage.createField("villageId");
    forgottenPasswordPageVillageId
        .name("Village Id")
        .type("Symbol")
        .localized(false)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const forgottenPasswordPageHeading = forgottenPasswordPage.createField("heading");
    forgottenPasswordPageHeading
        .name("Heading")
        .type("Symbol")
        .localized(true)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const forgottenPasswordPageBlock1 = forgottenPasswordPage.createField("block1");
    forgottenPasswordPageBlock1
        .name("Block 1")
        .type("Link")
        .localized(false)
        .required(false)
        .validations([{ "linkContentType": ["richTextBlock"] }])
        .disabled(false)
        .omitted(false)
        .linkType("Entry")

    const forgottenPasswordPageConfirmationBlock = forgottenPasswordPage.createField("confirmationBlock");
    forgottenPasswordPageConfirmationBlock
        .name("Confirmation Block")
        .type("Link")
        .localized(false)
        .required(false)
        .validations([{ "linkContentType": ["richTextBlock"] }])
        .disabled(false)
        .omitted(false)
        .linkType("Entry")

    const forgottenPasswordPageEmail = forgottenPasswordPage.createField("email");
    forgottenPasswordPageEmail
        .name("Email")
        .type("Link")
        .localized(true)
        .required(false)
        .validations([{ "linkContentType": ["formField"] }])
        .disabled(false)
        .omitted(false)
        .linkType("Entry")

    const forgottenPasswordPageSubmitText = forgottenPasswordPage.createField("submitText");
    forgottenPasswordPageSubmitText
        .name("Submit Text")
        .type("Symbol")
        .localized(true)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const forgottenPasswordPageServiceErrorMessage = forgottenPasswordPage.createField("serviceErrorMessage");
    forgottenPasswordPageServiceErrorMessage
        .name("Service Error Message")
        .type("Symbol")
        .localized(true)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)
    forgottenPasswordPage.changeFieldControl("id", "builtin", "singleLine")
    forgottenPasswordPage.changeFieldControl("villageId", "builtin", "singleLine")
    forgottenPasswordPage.changeFieldControl("heading", "builtin", "singleLine")
    forgottenPasswordPage.changeFieldControl("block1", "builtin", "entryLinkEditor")
    forgottenPasswordPage.changeFieldControl("confirmationBlock", "builtin", "entryLinkEditor")
    forgottenPasswordPage.changeFieldControl("email", "builtin", "entryLinkEditor")
    forgottenPasswordPage.changeFieldControl("submitText", "builtin", "singleLine")
    forgottenPasswordPage.changeFieldControl("serviceErrorMessage", "builtin", "singleLine")

    const profilePage = migration.createContentType("profilePage");
    profilePage
        .displayField("id")
        .name("Profile Page")
        .description("")

    const profilePageId = profilePage.createField("id");
    profilePageId
        .name("Id")
        .type("Symbol")
        .localized(false)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const profilePageVillageId = profilePage.createField("villageId");
    profilePageVillageId
        .name("Village Id")
        .type("Symbol")
        .localized(false)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const profilePageImage = profilePage.createField("image");
    profilePageImage
        .name("Image")
        .type("Link")
        .localized(false)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)
        .linkType("Asset")

    const profilePageMainImageForMobile = profilePage.createField("mainImageForMobile");
    profilePageMainImageForMobile
        .name("Main Image for Mobile")
        .type("Link")
        .localized(false)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)
        .linkType("Asset")

    const profilePageHeadingBlock = profilePage.createField("headingBlock");
    profilePageHeadingBlock
        .name("Heading Block")
        .type("Link")
        .localized(true)
        .required(false)
        .validations([{ "linkContentType": ["richTextBlock"] }])
        .disabled(false)
        .omitted(false)
        .linkType("Entry")

    const profilePageYourDetailsHeading = profilePage.createField("yourDetailsHeading");
    profilePageYourDetailsHeading
        .name("Your details heading")
        .type("Symbol")
        .localized(true)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const profilePageYourPreferencesHeading = profilePage.createField("yourPreferencesHeading");
    profilePageYourPreferencesHeading
        .name("Your preferences heading")
        .type("Symbol")
        .localized(true)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const profilePageNewEmail = profilePage.createField("newEmail");
    profilePageNewEmail
        .name("New Email")
        .type("Link")
        .localized(true)
        .required(false)
        .validations([{ "linkContentType": ["formField"] }])
        .disabled(false)
        .omitted(false)
        .linkType("Entry")

    const profilePageConfirmNewEmail = profilePage.createField("confirmNewEmail");
    profilePageConfirmNewEmail
        .name("Confirm New Email")
        .type("Link")
        .localized(true)
        .required(false)
        .validations([{ "linkContentType": ["formField"] }])
        .disabled(false)
        .omitted(false)
        .linkType("Entry")

    const profilePageCurrentPassword = profilePage.createField("currentPassword");
    profilePageCurrentPassword
        .name("Current Password")
        .type("Link")
        .localized(true)
        .required(false)
        .validations([{ "linkContentType": ["formField"] }])
        .disabled(false)
        .omitted(false)
        .linkType("Entry")

    const profilePageCloseButtonText = profilePage.createField("closeButtonText");
    profilePageCloseButtonText
        .name("Close Button Text")
        .type("Symbol")
        .localized(true)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const profilePageUpdateButtonText = profilePage.createField("updateButtonText");
    profilePageUpdateButtonText
        .name("Update Button Text")
        .type("Symbol")
        .localized(true)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const profilePageSubmitButtonText = profilePage.createField("submitButtonText");
    profilePageSubmitButtonText
        .name("Submit Button Text")
        .type("Symbol")
        .localized(true)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const profilePageSetPasswordUrl = profilePage.createField("setPasswordUrl");
    profilePageSetPasswordUrl
        .name("Set Password Url")
        .type("Symbol")
        .localized(false)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)
    profilePage.changeFieldControl("id", "builtin", "singleLine")
    profilePage.changeFieldControl("villageId", "builtin", "singleLine")
    profilePage.changeFieldControl("image", "builtin", "assetLinkEditor")
    profilePage.changeFieldControl("mainImageForMobile", "builtin", "assetLinkEditor")
    profilePage.changeFieldControl("headingBlock", "builtin", "entryLinkEditor")
    profilePage.changeFieldControl("yourDetailsHeading", "builtin", "singleLine")
    profilePage.changeFieldControl("yourPreferencesHeading", "builtin", "singleLine")
    profilePage.changeFieldControl("newEmail", "builtin", "entryLinkEditor")
    profilePage.changeFieldControl("confirmNewEmail", "builtin", "entryLinkEditor")
    profilePage.changeFieldControl("currentPassword", "builtin", "entryLinkEditor")
    profilePage.changeFieldControl("closeButtonText", "builtin", "singleLine")
    profilePage.changeFieldControl("updateButtonText", "builtin", "singleLine")
    profilePage.changeFieldControl("submitButtonText", "builtin", "singleLine")
    profilePage.changeFieldControl("setPasswordUrl", "builtin", "singleLine")

    const dashboardPage = migration.createContentType("dashboardPage");
    dashboardPage
        .displayField("id")
        .name("Dashboard Page")
        .description("")

    const dashboardPageId = dashboardPage.createField("id");
    dashboardPageId
        .name("Id")
        .type("Symbol")
        .localized(false)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const dashboardPageVillageId = dashboardPage.createField("villageId");
    dashboardPageVillageId
        .name("Village Id")
        .type("Symbol")
        .localized(false)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const dashboardPageGreeting = dashboardPage.createField("greeting");
    dashboardPageGreeting
        .name("Greeting ")
        .type("Symbol")
        .localized(true)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const dashboardPageIntroduction = dashboardPage.createField("introduction");
    dashboardPageIntroduction
        .name("Introduction ")
        .type("RichText")
        .localized(true)
        .required(false)
        .validations([{ "enabledMarks": ["bold", "italic", "underline", "code", "superscript", "subscript", "strikethrough"], "message": "Only bold, italic, underline, code, superscript, subscript, and strikethrough marks are allowed" }, { "enabledNodeTypes": ["heading-1", "heading-2", "heading-3", "heading-4", "heading-5", "heading-6", "ordered-list", "unordered-list", "hr", "blockquote", "embedded-entry-block", "embedded-asset-block", "table", "asset-hyperlink", "embedded-entry-inline", "entry-hyperlink", "hyperlink"], "message": "Only heading 1, heading 2, heading 3, heading 4, heading 5, heading 6, ordered list, unordered list, horizontal rule, quote, block entry, asset, table, link to asset, inline entry, link to entry, and link to Url nodes are allowed" }, { "nodes": {} }])
        .disabled(false)
        .omitted(false)

    const dashboardPageMembershipButtonText = dashboardPage.createField("membershipButtonText");
    dashboardPageMembershipButtonText
        .name("Membership button text")
        .type("Symbol")
        .localized(true)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const dashboardPageYourTreatsHeading = dashboardPage.createField("yourTreatsHeading");
    dashboardPageYourTreatsHeading
        .name("Your Treats heading")
        .type("Symbol")
        .localized(true)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const dashboardPageConfirmationMessageBody = dashboardPage.createField("confirmationMessageBody");
    dashboardPageConfirmationMessageBody
        .name("Confirmation Message Body")
        .type("RichText")
        .localized(true)
        .required(false)
        .validations([{ "enabledMarks": ["bold", "italic", "underline", "code", "superscript", "subscript", "strikethrough"], "message": "Only bold, italic, underline, code, superscript, subscript, and strikethrough marks are allowed" }, { "enabledNodeTypes": ["heading-1", "heading-2", "heading-3", "heading-4", "heading-5", "heading-6", "ordered-list", "unordered-list", "hr", "blockquote", "embedded-entry-block", "embedded-asset-block", "table", "asset-hyperlink", "embedded-entry-inline", "entry-hyperlink", "hyperlink"], "message": "Only heading 1, heading 2, heading 3, heading 4, heading 5, heading 6, ordered list, unordered list, horizontal rule, quote, block entry, asset, table, link to asset, inline entry, link to entry, and link to Url nodes are allowed" }, { "nodes": {} }])
        .disabled(false)
        .omitted(false)

    const dashboardPageYourEditHeading = dashboardPage.createField("yourEditHeading");
    dashboardPageYourEditHeading
        .name("Your Edit Heading")
        .type("Symbol")
        .localized(true)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const dashboardPageSeeAllTreatsText = dashboardPage.createField("seeAllTreatsText");
    dashboardPageSeeAllTreatsText
        .name("See All Treats Text")
        .type("Symbol")
        .localized(true)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const dashboardPageSeeAllEditsText = dashboardPage.createField("seeAllEditsText");
    dashboardPageSeeAllEditsText
        .name("See All Edits Text")
        .type("Symbol")
        .localized(true)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const dashboardPageWidgetHeading = dashboardPage.createField("widgetHeading");
    dashboardPageWidgetHeading
        .name("Widget heading ")
        .type("Symbol")
        .localized(true)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const dashboardPageWidgetSlotOne = dashboardPage.createField("widgetSlotOne");
    dashboardPageWidgetSlotOne
        .name("Widget Slot One")
        .type("Array")
        .localized(false)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)
        .items({ "type": "Link", "validations": [{ "linkContentType": ["iWidget"] }], "linkType": "Entry" })

    const dashboardPageFilterHeading = dashboardPage.createField("filterHeading");
    dashboardPageFilterHeading
        .name("Filter Heading")
        .type("Symbol")
        .localized(true)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const dashboardPageClearFilterHeading = dashboardPage.createField("clearFilterHeading");
    dashboardPageClearFilterHeading
        .name("Clear Filter Heading")
        .type("Symbol")
        .localized(true)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const dashboardPageStickyHeadingNoName = dashboardPage.createField("stickyHeadingNoName");
    dashboardPageStickyHeadingNoName
        .name("Sticky Heading No Name")
        .type("Symbol")
        .localized(true)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const dashboardPageStickyHeading = dashboardPage.createField("stickyHeading");
    dashboardPageStickyHeading
        .name("Sticky Heading")
        .type("Symbol")
        .localized(true)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const dashboardPageStickHeadingMobile = dashboardPage.createField("stickHeadingMobile");
    dashboardPageStickHeadingMobile
        .name("Stick Heading Mobile")
        .type("Symbol")
        .localized(true)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const dashboardPageStickyDescription = dashboardPage.createField("stickyDescription");
    dashboardPageStickyDescription
        .name("Sticky Description")
        .type("Symbol")
        .localized(true)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const dashboardPageReadMoreText = dashboardPage.createField("readMoreText");
    dashboardPageReadMoreText
        .name("Read More Text")
        .type("Symbol")
        .localized(true)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const dashboardPageTiersFooterText = dashboardPage.createField("tiersFooterText");
    dashboardPageTiersFooterText
        .name("Tiers Footer Text")
        .type("Symbol")
        .localized(true)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const dashboardPageBenefitsTitle = dashboardPage.createField("benefitsTitle");
    dashboardPageBenefitsTitle
        .name("Benefits Title")
        .type("Symbol")
        .localized(true)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const dashboardPageProgramTitle = dashboardPage.createField("programTitle");
    dashboardPageProgramTitle
        .name("Program Title")
        .type("Symbol")
        .localized(true)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const dashboardPageProgramDescription = dashboardPage.createField("programDescription");
    dashboardPageProgramDescription
        .name("Program Description")
        .type("Symbol")
        .localized(true)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const dashboardPageQrCodeTitle = dashboardPage.createField("qrCodeTitle");
    dashboardPageQrCodeTitle
        .name("QR Code Title")
        .type("Symbol")
        .localized(true)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const dashboardPageQrCodeDescription = dashboardPage.createField("qrCodeDescription");
    dashboardPageQrCodeDescription
        .name("QR Code Description")
        .type("Symbol")
        .localized(true)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const dashboardPageMembershipLevelText = dashboardPage.createField("membershipLevelText");
    dashboardPageMembershipLevelText
        .name("Membership Level Text")
        .type("Symbol")
        .localized(true)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const dashboardPagePointsText = dashboardPage.createField("pointsText");
    dashboardPagePointsText
        .name("Points Text")
        .type("Symbol")
        .localized(true)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const dashboardPagePanEuropeanRewardsTitle = dashboardPage.createField("panEuropeanRewardsTitle");
    dashboardPagePanEuropeanRewardsTitle
        .name("PanEuropean Rewards Title")
        .type("Symbol")
        .localized(true)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const dashboardPagePanEuropeanRewardsDescription = dashboardPage.createField("panEuropeanRewardsDescription");
    dashboardPagePanEuropeanRewardsDescription
        .name("PanEuropean Rewards Description")
        .type("Symbol")
        .localized(true)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const dashboardPageLevelResetsOn = dashboardPage.createField("levelResetsOn");
    dashboardPageLevelResetsOn
        .name("Level Resets On")
        .type("Symbol")
        .localized(true)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const dashboardPageStarsLeft = dashboardPage.createField("starsLeft");
    dashboardPageStarsLeft
        .name("Stars Left")
        .type("Symbol")
        .localized(true)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const dashboardPageStarsResetOn = dashboardPage.createField("starsResetOn");
    dashboardPageStarsResetOn
        .name("Stars Reset On")
        .type("Symbol")
        .localized(true)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)
    dashboardPage.changeFieldControl("id", "builtin", "singleLine")
    dashboardPage.changeFieldControl("villageId", "builtin", "singleLine")
    dashboardPage.changeFieldControl("greeting", "builtin", "singleLine")
    dashboardPage.changeFieldControl("introduction", "builtin", "richTextEditor")
    dashboardPage.changeFieldControl("membershipButtonText", "builtin", "singleLine")
    dashboardPage.changeFieldControl("yourTreatsHeading", "builtin", "singleLine")
    dashboardPage.changeFieldControl("confirmationMessageBody", "builtin", "richTextEditor")
    dashboardPage.changeFieldControl("yourEditHeading", "builtin", "singleLine")
    dashboardPage.changeFieldControl("seeAllTreatsText", "builtin", "singleLine")
    dashboardPage.changeFieldControl("seeAllEditsText", "builtin", "singleLine")
    dashboardPage.changeFieldControl("widgetHeading", "builtin", "singleLine")
    dashboardPage.changeFieldControl("widgetSlotOne", "builtin", "entryLinksEditor")
    dashboardPage.changeFieldControl("filterHeading", "builtin", "singleLine")
    dashboardPage.changeFieldControl("clearFilterHeading", "builtin", "singleLine")
    dashboardPage.changeFieldControl("stickyHeadingNoName", "builtin", "singleLine")
    dashboardPage.changeFieldControl("stickyHeading", "builtin", "singleLine")
    dashboardPage.changeFieldControl("stickHeadingMobile", "builtin", "singleLine")
    dashboardPage.changeFieldControl("stickyDescription", "builtin", "singleLine")
    dashboardPage.changeFieldControl("readMoreText", "builtin", "singleLine")
    dashboardPage.changeFieldControl("tiersFooterText", "builtin", "singleLine")
    dashboardPage.changeFieldControl("benefitsTitle", "builtin", "singleLine")
    dashboardPage.changeFieldControl("programTitle", "builtin", "singleLine")
    dashboardPage.changeFieldControl("programDescription", "builtin", "singleLine")
    dashboardPage.changeFieldControl("qrCodeTitle", "builtin", "singleLine")
    dashboardPage.changeFieldControl("qrCodeDescription", "builtin", "singleLine")
    dashboardPage.changeFieldControl("membershipLevelText", "builtin", "singleLine")
    dashboardPage.changeFieldControl("pointsText", "builtin", "singleLine")
    dashboardPage.changeFieldControl("panEuropeanRewardsTitle", "builtin", "singleLine")
    dashboardPage.changeFieldControl("panEuropeanRewardsDescription", "builtin", "singleLine")
    dashboardPage.changeFieldControl("levelResetsOn", "builtin", "singleLine")
    dashboardPage.changeFieldControl("starsLeft", "builtin", "singleLine")
    dashboardPage.changeFieldControl("starsResetOn", "builtin", "singleLine")

    const booleanField = migration.createContentType("booleanField");
    booleanField
        .displayField("label")
        .name("Boolean field")
        .description("")

    const booleanFieldLabel = booleanField.createField("label");
    booleanFieldLabel
        .name("Label")
        .type("Symbol")
        .localized(true)
        .required(true)
        .validations([])
        .disabled(false)
        .omitted(false)

    const booleanFieldValue = booleanField.createField("value");
    booleanFieldValue
        .name("Value")
        .type("Boolean")
        .localized(false)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)
    booleanField.changeFieldControl("label", "builtin", "singleLine")
    booleanField.changeFieldControl("value", "builtin", "boolean")

    const listField = migration.createContentType("listField");
    listField
        .displayField("label")
        .name("List Field")
        .description("")

    const listFieldLabel = listField.createField("label");
    listFieldLabel
        .name("Label")
        .type("Symbol")
        .localized(true)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const listFieldValue = listField.createField("value");
    listFieldValue
        .name("Value")
        .type("Array")
        .localized(false)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)
        .items({ "type": "Symbol", "validations": [] })

    const listFieldMandatoryWarning = listField.createField("mandatoryWarning");
    listFieldMandatoryWarning
        .name("Mandatory Warning")
        .type("Symbol")
        .localized(true)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const listFieldFormatWarning = listField.createField("formatWarning");
    listFieldFormatWarning
        .name("Format Warning")
        .type("Symbol")
        .localized(true)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)
    listField.changeFieldControl("label", "builtin", "singleLine")
    listField.changeFieldControl("value", "builtin", "tagEditor")
    listField.changeFieldControl("mandatoryWarning", "builtin", "singleLine")
    listField.changeFieldControl("formatWarning", "builtin", "singleLine")

    const birthdate = migration.createContentType("birthdate");
    birthdate
        .displayField("id")
        .name("Birthdate")
        .description("")

    const birthdateId = birthdate.createField("id");
    birthdateId
        .name("Id")
        .type("Symbol")
        .localized(false)
        .required(true)
        .validations([])
        .disabled(false)
        .omitted(false)

    const birthdateLabel = birthdate.createField("label");
    birthdateLabel
        .name("Label")
        .type("Symbol")
        .localized(true)
        .required(true)
        .validations([])
        .disabled(false)
        .omitted(false)

    const birthdateDay = birthdate.createField("day");
    birthdateDay
        .name("Day")
        .type("Integer")
        .localized(false)
        .required(false)
        .validations([{ "range": { "min": 1, "max": 31 } }])
        .disabled(false)
        .omitted(false)

    const birthdateMonth = birthdate.createField("month");
    birthdateMonth
        .name("Month")
        .type("Integer")
        .localized(false)
        .required(true)
        .validations([{ "range": { "min": 1, "max": 12 } }])
        .disabled(false)
        .omitted(false)

    const birthdateYear = birthdate.createField("year");
    birthdateYear
        .name("Year")
        .type("Integer")
        .localized(false)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const birthdateMandatoryWarning = birthdate.createField("mandatoryWarning");
    birthdateMandatoryWarning
        .name("Mandatory Warning")
        .type("Symbol")
        .localized(true)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const birthdateValidWarning = birthdate.createField("validWarning");
    birthdateValidWarning
        .name("Valid Warning")
        .type("Symbol")
        .localized(true)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)
    birthdate.changeFieldControl("id", "builtin", "singleLine")
    birthdate.changeFieldControl("label", "builtin", "singleLine")
    birthdate.changeFieldControl("day", "builtin", "numberEditor")
    birthdate.changeFieldControl("month", "builtin", "numberEditor")
    birthdate.changeFieldControl("year", "builtin", "numberEditor")
    birthdate.changeFieldControl("mandatoryWarning", "builtin", "singleLine")
    birthdate.changeFieldControl("validWarning", "builtin", "singleLine")

    const tcBlock = migration.createContentType("tcBlock");
    tcBlock
        .displayField("id")
        .name("T&C Block")
        .description("Terms and Conditions block")

    const tcBlockId = tcBlock.createField("id");
    tcBlockId
        .name("Id")
        .type("Symbol")
        .localized(false)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const tcBlockTermsAndConditionsLabel = tcBlock.createField("termsAndConditionsLabel");
    tcBlockTermsAndConditionsLabel
        .name("Terms and conditions label")
        .type("Symbol")
        .localized(true)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const tcBlockTermsMandatoryFieldWarning = tcBlock.createField("termsMandatoryFieldWarning");
    tcBlockTermsMandatoryFieldWarning
        .name("Terms mandatory field warning")
        .type("Symbol")
        .localized(true)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const tcBlockReceivingNewsletterLabel = tcBlock.createField("receivingNewsletterLabel");
    tcBlockReceivingNewsletterLabel
        .name("Receiving newsletter label")
        .type("Symbol")
        .localized(true)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const tcBlockTermsAndConditionsStatement = tcBlock.createField("termsAndConditionsStatement");
    tcBlockTermsAndConditionsStatement
        .name("Terms and conditions statement ")
        .type("RichText")
        .localized(true)
        .required(false)
        .validations([{ "enabledMarks": ["bold", "italic", "underline", "code", "superscript", "subscript", "strikethrough"], "message": "Only bold, italic, underline, code, superscript, subscript, and strikethrough marks are allowed" }, { "enabledNodeTypes": ["heading-1", "heading-2", "heading-3", "heading-4", "heading-5", "heading-6", "ordered-list", "unordered-list", "hr", "blockquote", "embedded-entry-block", "embedded-asset-block", "table", "asset-hyperlink", "embedded-entry-inline", "entry-hyperlink", "hyperlink"], "message": "Only heading 1, heading 2, heading 3, heading 4, heading 5, heading 6, ordered list, unordered list, horizontal rule, quote, block entry, asset, table, link to asset, inline entry, link to entry, and link to Url nodes are allowed" }, { "nodes": {} }])
        .disabled(false)
        .omitted(false)
    tcBlock.changeFieldControl("id", "builtin", "singleLine")
    tcBlock.changeFieldControl("termsAndConditionsLabel", "builtin", "singleLine")
    tcBlock.changeFieldControl("termsMandatoryFieldWarning", "builtin", "singleLine")
    tcBlock.changeFieldControl("receivingNewsletterLabel", "builtin", "singleLine")
    tcBlock.changeFieldControl("termsAndConditionsStatement", "builtin", "richTextEditor")

    const richTextBlock = migration.createContentType("richTextBlock");
    richTextBlock
        .displayField("id")
        .name("Rich Text Block")
        .description("Rich Text Block with heading and content in rich text")

    const richTextBlockId = richTextBlock.createField("id");
    richTextBlockId
        .name("Id")
        .type("Symbol")
        .localized(false)
        .required(true)
        .validations([])
        .disabled(false)
        .omitted(false)

    const richTextBlockHeading = richTextBlock.createField("heading");
    richTextBlockHeading
        .name("Heading")
        .type("Symbol")
        .localized(true)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const richTextBlockContent = richTextBlock.createField("content");
    richTextBlockContent
        .name("Content")
        .type("RichText")
        .localized(true)
        .required(false)
        .validations([{ "enabledMarks": ["bold", "italic", "underline", "code", "superscript", "subscript", "strikethrough"], "message": "Only bold, italic, underline, code, superscript, subscript, and strikethrough marks are allowed" }, { "enabledNodeTypes": ["heading-1", "heading-2", "heading-3", "heading-4", "heading-5", "heading-6", "ordered-list", "unordered-list", "hr", "blockquote", "embedded-entry-block", "embedded-asset-block", "table", "asset-hyperlink", "embedded-entry-inline", "entry-hyperlink", "hyperlink"], "message": "Only heading 1, heading 2, heading 3, heading 4, heading 5, heading 6, ordered list, unordered list, horizontal rule, quote, block entry, asset, table, link to asset, inline entry, link to entry, and link to Url nodes are allowed" }, { "nodes": {} }])
        .disabled(false)
        .omitted(false)
    richTextBlock.changeFieldControl("id", "builtin", "singleLine")
    richTextBlock.changeFieldControl("heading", "builtin", "singleLine")
    richTextBlock.changeFieldControl("content", "builtin", "richTextEditor")

    const registrationPage = migration.createContentType("registrationPage");
    registrationPage
        .displayField("id")
        .name("Registration Page")
        .description("")

    const registrationPageId = registrationPage.createField("id");
    registrationPageId
        .name("Id")
        .type("Symbol")
        .localized(false)
        .required(true)
        .validations([])
        .disabled(false)
        .omitted(false)

    const registrationPageVillageId = registrationPage.createField("villageId");
    registrationPageVillageId
        .name("Village Id")
        .type("Symbol")
        .localized(false)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const registrationPageIntroBlock = registrationPage.createField("introBlock");
    registrationPageIntroBlock
        .name("Intro Block")
        .type("Link")
        .localized(true)
        .required(false)
        .validations([{ "linkContentType": ["richTextBlock"] }])
        .disabled(false)
        .omitted(false)
        .linkType("Entry")

    const registrationPageBlock1 = registrationPage.createField("block1");
    registrationPageBlock1
        .name("Block 1")
        .type("Link")
        .localized(true)
        .required(false)
        .validations([{ "linkContentType": ["richTextBlock"] }])
        .disabled(false)
        .omitted(false)
        .linkType("Entry")

    const registrationPageBlock2 = registrationPage.createField("block2");
    registrationPageBlock2
        .name("Block 2")
        .type("Link")
        .localized(true)
        .required(false)
        .validations([{ "linkContentType": ["richTextBlock"] }])
        .disabled(false)
        .omitted(false)
        .linkType("Entry")

    const registrationPageTermsAndConditions = registrationPage.createField("termsAndConditions");
    registrationPageTermsAndConditions
        .name("Terms And Conditions")
        .type("Link")
        .localized(true)
        .required(false)
        .validations([{ "linkContentType": ["tcBlock"] }])
        .disabled(false)
        .omitted(false)
        .linkType("Entry")

    const registrationPageLeadSource = registrationPage.createField("leadSource");
    registrationPageLeadSource
        .name("Lead Source")
        .type("Symbol")
        .localized(false)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const registrationPageBypassRecaptcha = registrationPage.createField("bypassRecaptcha");
    registrationPageBypassRecaptcha
        .name("Bypass Recaptcha ")
        .type("Boolean")
        .localized(false)
        .required(false)
        .validations([])
        .defaultValue({ "en-US": false })
        .disabled(false)
        .omitted(false)

    const registrationPageEmail = registrationPage.createField("email");
    registrationPageEmail
        .name("Email")
        .type("Link")
        .localized(true)
        .required(true)
        .validations([{ "linkContentType": ["formField"] }])
        .disabled(false)
        .omitted(false)
        .linkType("Entry")

    const registrationPagePassword = registrationPage.createField("password");
    registrationPagePassword
        .name("Password")
        .type("Link")
        .localized(true)
        .required(true)
        .validations([{ "linkContentType": ["formField"] }])
        .disabled(false)
        .omitted(false)
        .linkType("Entry")

    const registrationPageConfirmPassword = registrationPage.createField("confirmPassword");
    registrationPageConfirmPassword
        .name("Confirm Password")
        .type("Link")
        .localized(true)
        .required(true)
        .validations([{ "linkContentType": ["formField"] }])
        .disabled(false)
        .omitted(false)
        .linkType("Entry")

    const registrationPageFirstName = registrationPage.createField("firstName");
    registrationPageFirstName
        .name("First Name")
        .type("Link")
        .localized(true)
        .required(true)
        .validations([{ "linkContentType": ["formField"] }])
        .disabled(false)
        .omitted(false)
        .linkType("Entry")

    const registrationPageLastName = registrationPage.createField("lastName");
    registrationPageLastName
        .name("Last Name")
        .type("Link")
        .localized(true)
        .required(true)
        .validations([{ "linkContentType": ["formField"] }])
        .disabled(false)
        .omitted(false)
        .linkType("Entry")

    const registrationPageBirthDate = registrationPage.createField("birthDate");
    registrationPageBirthDate
        .name("BirthDate")
        .type("Link")
        .localized(true)
        .required(false)
        .validations([{ "linkContentType": ["birthdate"] }])
        .disabled(false)
        .omitted(false)
        .linkType("Entry")

    const registrationPageMobile = registrationPage.createField("mobile");
    registrationPageMobile
        .name("Mobile")
        .type("Link")
        .localized(true)
        .required(false)
        .validations([{ "linkContentType": ["formField"] }])
        .disabled(false)
        .omitted(false)
        .linkType("Entry")

    const registrationPageGender = registrationPage.createField("gender");
    registrationPageGender
        .name("Gender")
        .type("Link")
        .localized(true)
        .required(false)
        .validations([{ "linkContentType": ["listField"] }])
        .disabled(false)
        .omitted(false)
        .linkType("Entry")

    const registrationPageCountryOfResidence = registrationPage.createField("countryOfResidence");
    registrationPageCountryOfResidence
        .name("Country of Residence")
        .type("Link")
        .localized(true)
        .required(false)
        .validations([{ "linkContentType": ["listField"] }])
        .disabled(false)
        .omitted(false)
        .linkType("Entry")

    const registrationPageCountry = registrationPage.createField("country");
    registrationPageCountry
        .name("Country")
        .type("Link")
        .localized(true)
        .required(false)
        .validations([{ "linkContentType": ["listField"] }])
        .disabled(false)
        .omitted(false)
        .linkType("Entry")

    const registrationPagePostcode = registrationPage.createField("postcode");
    registrationPagePostcode
        .name("Postcode")
        .type("Link")
        .localized(true)
        .required(false)
        .validations([{ "linkContentType": ["formField"] }])
        .disabled(false)
        .omitted(false)
        .linkType("Entry")

    const registrationPageConsentMobileChannels = registrationPage.createField("consentMobileChannels");
    registrationPageConsentMobileChannels
        .name("Consent Mobile Channels")
        .type("Link")
        .localized(true)
        .required(false)
        .validations([{ "linkContentType": ["booleanField"] }])
        .disabled(false)
        .omitted(false)
        .linkType("Entry")

    const registrationPageConsentSms = registrationPage.createField("consentSms");
    registrationPageConsentSms
        .name("Consent Sms")
        .type("Link")
        .localized(true)
        .required(false)
        .validations([{ "linkContentType": ["booleanField"] }])
        .disabled(false)
        .omitted(false)
        .linkType("Entry")

    const registrationPageConsentPost = registrationPage.createField("consentPost");
    registrationPageConsentPost
        .name("Consent Post")
        .type("Link")
        .localized(true)
        .required(false)
        .validations([{ "linkContentType": ["booleanField"] }])
        .disabled(false)
        .omitted(false)
        .linkType("Entry")

    const registrationPageConsentPhone = registrationPage.createField("consentPhone");
    registrationPageConsentPhone
        .name("Consent Phone")
        .type("Link")
        .localized(true)
        .required(false)
        .validations([{ "linkContentType": ["booleanField"] }])
        .disabled(false)
        .omitted(false)
        .linkType("Entry")

    const registrationPageSubmitText = registrationPage.createField("submitText");
    registrationPageSubmitText
        .name("Submit Text")
        .type("Symbol")
        .localized(true)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const registrationPageServiceErrorMessage = registrationPage.createField("serviceErrorMessage");
    registrationPageServiceErrorMessage
        .name("Service Error Message")
        .type("Symbol")
        .localized(true)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)
    registrationPage.changeFieldControl("id", "builtin", "singleLine")
    registrationPage.changeFieldControl("villageId", "builtin", "singleLine")
    registrationPage.changeFieldControl("introBlock", "builtin", "entryLinkEditor")
    registrationPage.changeFieldControl("block1", "builtin", "entryLinkEditor")
    registrationPage.changeFieldControl("block2", "builtin", "entryLinkEditor")
    registrationPage.changeFieldControl("termsAndConditions", "builtin", "entryLinkEditor")
    registrationPage.changeFieldControl("leadSource", "builtin", "singleLine")
    registrationPage.changeFieldControl("bypassRecaptcha", "builtin", "boolean")
    registrationPage.changeFieldControl("email", "builtin", "entryLinkEditor")
    registrationPage.changeFieldControl("password", "builtin", "entryLinkEditor")
    registrationPage.changeFieldControl("confirmPassword", "builtin", "entryLinkEditor")
    registrationPage.changeFieldControl("firstName", "builtin", "entryLinkEditor")
    registrationPage.changeFieldControl("lastName", "builtin", "entryLinkEditor")
    registrationPage.changeFieldControl("birthDate", "builtin", "entryLinkEditor")
    registrationPage.changeFieldControl("mobile", "builtin", "entryLinkEditor")
    registrationPage.changeFieldControl("gender", "builtin", "entryLinkEditor")
    registrationPage.changeFieldControl("countryOfResidence", "builtin", "entryLinkEditor")
    registrationPage.changeFieldControl("country", "builtin", "entryLinkEditor")
    registrationPage.changeFieldControl("postcode", "builtin", "entryLinkEditor")
    registrationPage.changeFieldControl("consentMobileChannels", "builtin", "entryLinkEditor")
    registrationPage.changeFieldControl("consentSms", "builtin", "entryLinkEditor")
    registrationPage.changeFieldControl("consentPost", "builtin", "entryLinkEditor")
    registrationPage.changeFieldControl("consentPhone", "builtin", "entryLinkEditor")
    registrationPage.changeFieldControl("submitText", "builtin", "singleLine")
    registrationPage.changeFieldControl("serviceErrorMessage", "builtin", "singleLine")

    const signInPage = migration.createContentType("signInPage");
    signInPage
        .displayField("id")
        .name("Sign In Page")
        .description("")

    const signInPageId = signInPage.createField("id");
    signInPageId
        .name("Id")
        .type("Symbol")
        .localized(false)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const signInPageVillageId = signInPage.createField("villageId");
    signInPageVillageId
        .name("Village ID")
        .type("Symbol")
        .localized(false)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const signInPageImageForMobile = signInPage.createField("imageForMobile");
    signInPageImageForMobile
        .name("Image For Mobile")
        .type("Link")
        .localized(true)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)
        .linkType("Asset")

    const signInPagePageHeading = signInPage.createField("pageHeading");
    signInPagePageHeading
        .name("Page Heading")
        .type("Symbol")
        .localized(true)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const signInPageIntroduction = signInPage.createField("introduction");
    signInPageIntroduction
        .name("Introduction")
        .type("RichText")
        .localized(true)
        .required(false)
        .validations([{ "enabledMarks": ["bold", "italic", "underline", "code", "superscript", "subscript", "strikethrough"], "message": "Only bold, italic, underline, code, superscript, subscript, and strikethrough marks are allowed" }, { "enabledNodeTypes": ["heading-1", "heading-2", "heading-3", "heading-4", "heading-5", "heading-6", "ordered-list", "unordered-list", "hr", "blockquote", "embedded-entry-block", "embedded-asset-block", "table", "asset-hyperlink", "embedded-entry-inline", "entry-hyperlink", "hyperlink"], "message": "Only heading 1, heading 2, heading 3, heading 4, heading 5, heading 6, ordered list, unordered list, horizontal rule, quote, block entry, asset, table, link to asset, inline entry, link to entry, and link to Url nodes are allowed" }, { "nodes": {} }])
        .disabled(false)
        .omitted(false)

    const signInPageEmail = signInPage.createField("email");
    signInPageEmail
        .name("Email")
        .type("Link")
        .localized(true)
        .required(false)
        .validations([{ "linkContentType": ["formField"] }])
        .disabled(false)
        .omitted(false)
        .linkType("Entry")

    const signInPagePassword = signInPage.createField("password");
    signInPagePassword
        .name("Password")
        .type("Link")
        .localized(true)
        .required(false)
        .validations([{ "linkContentType": ["formField"] }])
        .disabled(false)
        .omitted(false)
        .linkType("Entry")

    const signInPageForgottenPassword = signInPage.createField("forgottenPassword");
    signInPageForgottenPassword
        .name("Forgotten Password")
        .type("Link")
        .localized(true)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)
        .linkType("Entry")

    const signInPageRememberMe = signInPage.createField("rememberMe");
    signInPageRememberMe
        .name("Remember Me")
        .type("Symbol")
        .localized(true)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const signInPageBypassRecaptcha = signInPage.createField("bypassRecaptcha");
    signInPageBypassRecaptcha
        .name("Bypass Recaptcha")
        .type("Boolean")
        .localized(false)
        .required(false)
        .validations([])
        .defaultValue({ "en-US": false })
        .disabled(false)
        .omitted(false)

    const signInPageLeadSource = signInPage.createField("leadSource");
    signInPageLeadSource
        .name("Lead Source")
        .type("Symbol")
        .localized(false)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const signInPageSuspendedAccountMessage = signInPage.createField("suspendedAccountMessage");
    signInPageSuspendedAccountMessage
        .name("Suspended Account Message")
        .type("Symbol")
        .localized(true)
        .required(true)
        .validations([])
        .disabled(false)
        .omitted(false)

    const signInPageBlockedAccountMessage = signInPage.createField("blockedAccountMessage");
    signInPageBlockedAccountMessage
        .name("Blocked Account Message")
        .type("Symbol")
        .localized(true)
        .required(true)
        .validations([])
        .disabled(false)
        .omitted(false)

    const signInPageServiceErrorMessage = signInPage.createField("serviceErrorMessage");
    signInPageServiceErrorMessage
        .name("Service Error Message")
        .type("Symbol")
        .localized(true)
        .required(true)
        .validations([])
        .disabled(false)
        .omitted(false)

    const signInPageSubmitText = signInPage.createField("submitText");
    signInPageSubmitText
        .name("Submit Text")
        .type("Symbol")
        .localized(true)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)
    signInPage.changeFieldControl("id", "builtin", "singleLine")
    signInPage.changeFieldControl("villageId", "builtin", "singleLine")
    signInPage.changeFieldControl("imageForMobile", "builtin", "assetLinkEditor")
    signInPage.changeFieldControl("pageHeading", "builtin", "singleLine")
    signInPage.changeFieldControl("introduction", "builtin", "richTextEditor")
    signInPage.changeFieldControl("email", "builtin", "entryLinkEditor")
    signInPage.changeFieldControl("password", "builtin", "entryLinkEditor")
    signInPage.changeFieldControl("forgottenPassword", "builtin", "entryLinkEditor")
    signInPage.changeFieldControl("rememberMe", "builtin", "singleLine")
    signInPage.changeFieldControl("bypassRecaptcha", "builtin", "boolean")
    signInPage.changeFieldControl("leadSource", "builtin", "singleLine")
    signInPage.changeFieldControl("suspendedAccountMessage", "builtin", "singleLine")
    signInPage.changeFieldControl("blockedAccountMessage", "builtin", "singleLine")
    signInPage.changeFieldControl("serviceErrorMessage", "builtin", "singleLine")
    signInPage.changeFieldControl("submitText", "builtin", "singleLine")

    const formField = migration.createContentType("formField");
    formField
        .displayField("label")
        .name("Form Field")
        .description("")

    const formFieldLabel = formField.createField("label");
    formFieldLabel
        .name("Label")
        .type("Symbol")
        .localized(false)
        .required(true)
        .validations([])
        .disabled(false)
        .omitted(false)

    const formFieldValue = formField.createField("value");
    formFieldValue
        .name("Value")
        .type("Symbol")
        .localized(false)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const formFieldMandatoryWarning = formField.createField("mandatoryWarning");
    formFieldMandatoryWarning
        .name("Mandatory Warning")
        .type("Symbol")
        .localized(false)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const formFieldFormatWarning = formField.createField("formatWarning");
    formFieldFormatWarning
        .name("Format Warning")
        .type("Symbol")
        .localized(false)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)
    formField.changeFieldControl("label", "builtin", "singleLine")
    formField.changeFieldControl("value", "builtin", "singleLine")
    formField.changeFieldControl("mandatoryWarning", "builtin", "singleLine")
    formField.changeFieldControl("formatWarning", "builtin", "singleLine")

    const layout = migration.createContentType("layout");
    layout
        .name("Layout")
        .description("Layout page that holds Header and Footer and wraps inside the rendered page requested")

    const layoutHeader = layout.createField("header");
    layoutHeader
        .name("Header")
        .type("Link")
        .localized(false)
        .required(false)
        .validations([{ "linkContentType": ["header"] }])
        .disabled(false)
        .omitted(false)
        .linkType("Entry")

    const layoutFooter = layout.createField("footer");
    layoutFooter
        .name("Footer")
        .type("Link")
        .localized(false)
        .required(false)
        .validations([{ "linkContentType": ["footer"] }])
        .disabled(false)
        .omitted(false)
        .linkType("Entry")
    layout.changeFieldControl("header", "builtin", "entryLinkEditor")
    layout.changeFieldControl("footer", "builtin", "entryLinkEditor")

    const footerColumn = migration.createContentType("footerColumn");
    footerColumn
        .displayField("name")
        .name("Footer Column")
        .description("")

    const footerColumnName = footerColumn.createField("name");
    footerColumnName
        .name("Name")
        .type("Symbol")
        .localized(false)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const footerColumnLinks = footerColumn.createField("links");
    footerColumnLinks
        .name("Links")
        .type("Array")
        .localized(false)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)
        .items({ "type": "Link", "validations": [{ "linkContentType": ["basicLink"] }], "linkType": "Entry" })
    footerColumn.changeFieldControl("name", "builtin", "singleLine")
    footerColumn.changeFieldControl("links", "builtin", "entryLinksEditor")

    const icp = migration.createContentType("icp");
    icp
        .displayField("label")
        .name("ICP")
        .description("")

    const icpLabel = icp.createField("label");
    icpLabel
        .name("Label")
        .type("Symbol")
        .localized(false)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const icpPolicyLink = icp.createField("policyLink");
    icpPolicyLink
        .name("Policy Link")
        .type("Link")
        .localized(false)
        .required(false)
        .validations([{ "linkContentType": ["basicLink"] }])
        .disabled(false)
        .omitted(false)
        .linkType("Entry")
    icp.changeFieldControl("label", "builtin", "singleLine", { "helpText": "ICP Label" })
    icp.changeFieldControl("policyLink", "builtin", "entryLinkEditor")

    const footer = migration.createContentType("footer");
    footer
        .displayField("name")
        .name("Footer")
        .description("")

    const footerName = footer.createField("name");
    footerName
        .name("Name")
        .type("Symbol")
        .localized(false)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)

    const footerLogo = footer.createField("logo");
    footerLogo
        .name("Logo")
        .type("Link")
        .localized(false)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)
        .linkType("Asset")

    const footerFirstColumn = footer.createField("firstColumn");
    footerFirstColumn
        .name("First Column")
        .type("Link")
        .localized(false)
        .required(false)
        .validations([{ "linkContentType": ["footerColumn"] }])
        .disabled(false)
        .omitted(false)
        .linkType("Entry")

    const footerSecondColumn = footer.createField("secondColumn");
    footerSecondColumn
        .name("Second Column")
        .type("Link")
        .localized(false)
        .required(false)
        .validations([{ "linkContentType": ["footerColumn"] }])
        .disabled(false)
        .omitted(false)
        .linkType("Entry")

    const footerSocialMediaLinks = footer.createField("socialMediaLinks");
    footerSocialMediaLinks
        .name("Social Media Links")
        .type("Array")
        .localized(false)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)
        .items({ "type": "Link", "validations": [{ "linkContentType": ["basicLink"] }], "linkType": "Entry" })

    const footerFooterLegalNotice = footer.createField("footerLegalNotice");
    footerFooterLegalNotice
        .name("Footer Legal Notice")
        .type("RichText")
        .localized(false)
        .required(false)
        .validations([{ "enabledMarks": ["bold", "italic", "underline", "code", "superscript", "subscript", "strikethrough"], "message": "Only bold, italic, underline, code, superscript, subscript, and strikethrough marks are allowed" }, { "enabledNodeTypes": ["heading-1", "heading-2", "heading-3", "heading-4", "heading-5", "heading-6", "ordered-list", "unordered-list", "hr", "blockquote", "embedded-entry-block", "embedded-asset-block", "table", "asset-hyperlink", "embedded-entry-inline", "entry-hyperlink", "hyperlink"], "message": "Only heading 1, heading 2, heading 3, heading 4, heading 5, heading 6, ordered list, unordered list, horizontal rule, quote, block entry, asset, table, link to asset, inline entry, link to entry, and link to Url nodes are allowed" }, { "nodes": {} }])
        .disabled(false)
        .omitted(false)

    const footerFooterCopyrightText = footer.createField("footerCopyrightText");
    footerFooterCopyrightText
        .name("Footer Copyright Text")
        .type("RichText")
        .localized(false)
        .required(false)
        .validations([{ "enabledMarks": ["bold", "italic", "underline", "code", "superscript", "subscript", "strikethrough"], "message": "Only bold, italic, underline, code, superscript, subscript, and strikethrough marks are allowed" }, { "enabledNodeTypes": ["heading-1", "heading-2", "heading-3", "heading-4", "heading-5", "heading-6", "ordered-list", "unordered-list", "hr", "blockquote", "embedded-entry-block", "embedded-asset-block", "table", "asset-hyperlink", "embedded-entry-inline", "entry-hyperlink", "hyperlink"], "message": "Only heading 1, heading 2, heading 3, heading 4, heading 5, heading 6, ordered list, unordered list, horizontal rule, quote, block entry, asset, table, link to asset, inline entry, link to entry, and link to Url nodes are allowed" }, { "nodes": {} }])
        .disabled(false)
        .omitted(false)

    const footerIcp = footer.createField("icp");
    footerIcp
        .name("ICP")
        .type("Link")
        .localized(false)
        .required(false)
        .validations([{ "linkContentType": ["icp"] }])
        .disabled(false)
        .omitted(false)
        .linkType("Entry")

    const footerThirdColumn = footer.createField("thirdColumn");
    footerThirdColumn
        .name("Third Column")
        .type("Link")
        .localized(false)
        .required(false)
        .validations([{ "linkContentType": ["footerColumn"] }])
        .disabled(false)
        .omitted(false)
        .linkType("Entry")
    footer.changeFieldControl("name", "builtin", "singleLine")
    footer.changeFieldControl("logo", "builtin", "assetLinkEditor", { "helpText": "Footer Logo", "showLinkEntityAction": true, "showCreateEntityAction": true })
    footer.changeFieldControl("firstColumn", "builtin", "entryLinkEditor")
    footer.changeFieldControl("secondColumn", "builtin", "entryLinkEditor")
    footer.changeFieldControl("socialMediaLinks", "builtin", "entryLinksEditor")
    footer.changeFieldControl("footerLegalNotice", "builtin", "richTextEditor")
    footer.changeFieldControl("footerCopyrightText", "builtin", "richTextEditor")
    footer.changeFieldControl("icp", "builtin", "entryLinkEditor")
    footer.changeFieldControl("thirdColumn", "builtin", "entryLinkEditor")

    const header = migration.createContentType("header");
    header
        .name("Header")
        .description("")

    const headerLogo = header.createField("logo");
    headerLogo
        .name("Logo")
        .type("Link")
        .localized(false)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)
        .linkType("Asset")

    const headerMenuLinks = header.createField("menuLinks");
    headerMenuLinks
        .name("Menu Links")
        .type("Array")
        .localized(false)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)
        .items({ "type": "Link", "validations": [{ "linkContentType": ["basicLink"] }], "linkType": "Entry" })

    const headerLanguageLinks = header.createField("languageLinks");
    headerLanguageLinks
        .name("Language Links")
        .type("Array")
        .localized(false)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)
        .items({ "type": "Link", "validations": [{ "linkContentType": ["basicLink"] }], "linkType": "Entry" })

    const headerNavigationLinks = header.createField("navigationLinks");
    headerNavigationLinks
        .name("Navigation Links")
        .type("Array")
        .localized(false)
        .required(false)
        .validations([])
        .disabled(false)
        .omitted(false)
        .items({ "type": "Link", "validations": [{ "linkContentType": ["basicLink"] }], "linkType": "Entry" })
    header.changeFieldControl("logo", "builtin", "assetLinkEditor", { "helpText": "Village logo", "showLinkEntityAction": true, "showCreateEntityAction": true })
    header.changeFieldControl("menuLinks", "builtin", "entryLinksEditor", { "helpText": "Links for header menu", "bulkEditing": false, "showLinkEntityAction": true, "showCreateEntityAction": true })
    header.changeFieldControl("languageLinks", "builtin", "entryLinksEditor", { "helpText": "Links for language selection menu", "bulkEditing": false, "showLinkEntityAction": true, "showCreateEntityAction": true })
    header.changeFieldControl("navigationLinks", "builtin", "entryLinksEditor", { "helpText": "Links for navigation menu", "bulkEditing": false, "showLinkEntityAction": true, "showCreateEntityAction": true })

    const basicLink = migration.createContentType("basicLink");
    basicLink
        .displayField("text")
        .name("BasicLink")
        .description("Holds links information")

    const basicLinkText = basicLink.createField("text");
    basicLinkText
        .name("Text")
        .type("Symbol")
        .localized(false)
        .required(true)
        .validations([])
        .disabled(false)
        .omitted(false)

    const basicLinkUrl = basicLink.createField("url");
    basicLinkUrl
        .name("Url")
        .type("Symbol")
        .localized(false)
        .required(true)
        .validations([])
        .disabled(false)
        .omitted(false)

    const basicLinkNewWindow = basicLink.createField("newWindow");
    basicLinkNewWindow
        .name("NewWindow")
        .type("Boolean")
        .localized(false)
        .required(false)
        .validations([])
        .defaultValue({ "en-US": true })
        .disabled(false)
        .omitted(false)

    const basicLinkRequireAuth = basicLink.createField("requireAuth");
    basicLinkRequireAuth
        .name("RequireAuth")
        .type("Boolean")
        .localized(false)
        .required(false)
        .validations([])
        .defaultValue({ "en-US": false })
        .disabled(false)
        .omitted(false)

    const basicLinkIsSelected = basicLink.createField("isSelected");
    basicLinkIsSelected
        .name("IsSelected")
        .type("Boolean")
        .localized(false)
        .required(false)
        .validations([])
        .defaultValue({ "en-US": false })
        .disabled(false)
        .omitted(false)
    basicLink.changeFieldControl("text", "builtin", "singleLine")
    basicLink.changeFieldControl("url", "builtin", "singleLine")
    basicLink.changeFieldControl("newWindow", "builtin", "boolean")
    basicLink.changeFieldControl("requireAuth", "builtin", "boolean")
    basicLink.changeFieldControl("isSelected", "builtin", "boolean")
}
module.exports = migrationFunction;
