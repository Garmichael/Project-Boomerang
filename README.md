# Project-Boomerang
A Unity3D engine project for creating side-scroller and top-down games


User created content goes in the appropriate folders in the Resources Directory. Everything else will eventually be handled by the content inside the Retro Game Kit folder.

Right now, Player Controller States, Camera States, Map Areas, Map Data, and Tile Sheet Data can be edited and the engine will automatically parse those changes.

## Player Controller and Player States

Create Player Controllers in the Assets/Resources/Player folder. There are two demo controllers already there that can be used. They extend the BaseCharacterController class. 

When creating a Player Controller, use the Start method to set properties unique to that controller. Also register the PlayerStates appropriate for it. Don't forget to call the base Start method afterwards.

### Adding Player States

In your controller, first create a MovementStateManager and a CombatStateManager. They're effectively the same thing, but having two state managers allows you to have two concurrent states (For example: Running + Shooting). 

Afterwards, you have to register the available states for your controller. Each state is an instanced object tied to your controller manager.

> MovementStateManager = new StateManager("Player Movement State Manager", this.gameObject);

> MovementStateManager.AddState(new PlayerState_Idle(MovementStateManager));

#### Creating Player States

Player States are objects that extend BaseState. There are four methods for your State:

> OnEnterState(), OnExitState()

These are automatically called when you switch states

> ProcessState()

This is called near the beginning of the BaseCharacterController class.

> ProcessPostFrameState()

This is called after processing changes from ProcessState().

##### Writing States

The primary use of a State is to manage player input and physics for that state.

Most of your logic should be in the ProcessState() method.

You should not modify the position of the player in a state, calculate collisions, or apply gravity. All of this is done inside the BaseCharacterController. You only have to adjust the Velocity.x and Velocity.y. In this way, if you create a walking state, you only have to worry about adding Velocity.x. 

When initializing the State, you pass in a reference to the StateManager (see above under Adding Player States). With this rerference, you have the ability to change the current state. Use MyStateManager.SetNextState("state name") to change states. Note that the state will not happen immediately. The BaseCharacterController will update the state at the end of the current frame. By doing this, you can easily switch from to a walking state directly inside idle state by listening to input by the player.

Sometimes, you want your state to do something special at the end of the frame, after the BaseCharacterController has applied the velocity set in ProcessState(). Put this logic inside ProcessPostFrameState().

There are a number of existing states to use as reference.

##### Camera States

The CameraController In Assets/Resources/RetroGameKit/Camera. Later, this will be a base class, and camera controllers will extend from it. For now, there is just one Camera Controller. You register your classes in this controller, and they work just like PlayerStates do.


## Building Maps

There are three levels of maps: Areas, Maps, and Views.

### Areas

Areas are made out of multiple Maps.

Place Area definition files in Assets/Resources/Maps/Area Data.

There is an example file in this directory already. Use it to understand the format. Backgrounds and Foregrounds do not yet currently work.

Each Map layer has three properties: Name, X-Start, and Y-Start.

Areas are loaded with the following command (executable from anywhere in your project):

> AreaBuilder.BuildArea("area name");

The Area name matches the filename of your .xml file. For example, to load MyAwesomeArea.xml, use AreaBuilder.BuildArea("MyAwesomeArea");


### Maps

Maps belong in the Assets/Resources/Maps/Map Data directory.

There is an example Map in the folder called DemoMap. The format for a Map file is: [mapName]_properties.xml. [mapName] is references in your Area.xml file under Name. For example, for an Area to load MyAwesomeMap_properties.xml, simply put <Name>MyAwesomeMap</Name> In your MyAwesomeArea.xml.

Maps have Layers and Views.

##### Layers

There are 10 available slots of Layers, referenced by the LayerId property. 

Each Layer also has a name. This referencces a CSV file in the same folder called [mapName]_[layerName].txt.

For example, <Name>MyAwesomeLayer</Name> will look for MyAwesomeMap_MyAwesomeLayer.txt.

The Tilesheet property references a tilesheet to use.

The UseColliders property is a flag that tells the MapBuilder whether or not to add colliders to tiles. This can be used to create foreground and background tiles that the player doesn't interat with, including false illusionary walls.

##### Views

Views are rectangular areas that define camera behavior.

The Camera-Transition-Mode property defines what the camera automatically does when the player Enters that view. Currently only "blip" is implemented, which causes the camera to teleport into the new view. "pan" will freeze the action and pan the camera into the new view, but it is not yet implemented.

The start and end properties define the boundaries of the view.

To be implemented still is a property that defines the default Camera State to use when the player enters the view.

### Tilesets

Tilesets are made out of two files: an image and a collisions file.

Both files are placed in the Assets/Resources/Maps/Tilesheets directory.


##### Tileset Image

You can use any image file to define the graphical component of the tileset. It's important that you set the image properties correctly from within the Unity Editor.

> Texture Type: Sprite (2D and UI)

> Sprite Mode: Multiple

Make sure you slice your tileset using the Unity Editor's tool for doing so.

Depending on the Tile Editor you use, the top left tile may need to be the "empty tile" graphic. Unity ignores empty tiles when slicing, so it's advised that you set a very transparent, virtually invisible tile in the slot so that Unity recognizes it, but your external csv tile map editor appears as though it's blank.

##### Properties file

The property file must be named the same as your image, except with _collisions added to the end. For example:

> MyAwesomeTileSheet.png

> MyAwesomeTileSheet_collisions

This property file maps your tile IDs to properties of the tile. There are a number of options. Check out the existing file in that directory for more information on formatting the file and what properties are available.

Note that you should not have any spaces in this file anywhere. For example, a space between the colon after the property name will cause an exception error.

To use your tilesheet in your map, simply reference the name of your file in your map properties file.

