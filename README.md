# ExampleMod
## Example mod for Inscryption API made by Cyantist

This plugin is a BepInEx plugin made for Inscryption to create custom cards for the API by Cyantist.
One example card is provided in the Plugin class with method name AddBears.
One customised card is provided in the Plugin class with method name ChangeWolf.

## Installation
To install this plugin first you need to install BepInEx as a mod loader for Inscryption. A guide to do this can be found [here](https://docs.bepinex.dev/articles/user_guide/installation/index.html#where-to-download-bepinex). Inscryption needs the 86x (32 bit) mono version.

You will also need to install the [CardLoaderPlugin](https://github.com/ScottWilson0903/InscryptionAPI)
To install ExampleMod with just the default example card included "Eight Fucking Bears!" and example modified wolf, you simply need to copy **API.dll** from [releases](https://github.com/ScottWilson0903/InscryptionAPI/releases) and the **Artwork** folder from the source code zip in [releases](https://github.com/ScottWilson0903/InscryptionAPI/releases) to a **CardLoader** folder inside **Inscryption/BepInEx/plugins**.

To generate your own cards, you will need to either pass in an already created **CardInfo** object to the **NewCard.Add** function, or you will need to pass all the required and optional parameters to the **NewCard** constructor as done in **Plugin.AddBears**. Any png files should be added to the **Artwork** folder and should be 114x94 pixels.
To alter existing cards you will need to pass the card name and the values you want to change to the optional parameters in the **CustomCard** constructor, as done in **Plugin.ChangeWolf**.
To add custom abilities you will need a class inheriting **AbilityBehaviour**, an **AbilityInfo** and a texture which should be 49x49 pixels and also placed in the **Artwork** folder.

The newly compiled **CardLoaderMod.dll** should be installed exactly the same way as above.
