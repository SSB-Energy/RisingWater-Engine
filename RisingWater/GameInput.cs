using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using Raylib_cs;
using System.Reflection;
using System.Collections;

namespace RisingWater
{
    /// <summary>
    /// JSON-serializable controls map.
    /// </summary>
    public class ControlsMap
    {
        public string[] up { get; set; }
        public string[] down { get; set; }
        public string[] left { get; set; }
        public string[] right { get; set; }
        public string[] jump { get; set; }

        /// <summary>
        /// JSON-style access for the JSON-style object
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public string[] this[string propertyName]
        {
            get
            {
                PropertyInfo property = GetType().GetProperty(propertyName);
                return (string[])property.GetValue(this, null);
            }
        }
    }

    public class ControlsContainer
    {
        public ControlsMap keyboardControls { get; set; }
        public ControlsMap gamepadControls { get; set; }

        public float deadzone { get; set; }

        /// <summary>
        /// JSON-style access for the JSON-style object
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public ControlsMap this[string propertyName]
        {
            get
            {
                PropertyInfo property = GetType().GetProperty(propertyName);
                return (ControlsMap)property.GetValue(this, null);
            }
        }
    }

    public enum GamepadBool
    {
        LSTICK_UP,
        LSTICK_DOWN,
        LSTICK_LEFT,
        LSTICK_RIGHT,
        RSTICK_UP,
        RSTICK_DOWN,
        RSTICK_LEFT,
        RSTICK_RIGHT,
        DPAD_UP,
        DPAD_DOWN,
        DPAD_LEFT,
        DPAD_RIGHT,
        XBOX_A,
        XBOX_B,
        XBOX_X,
        XBOX_Y,
        PS_X,
        PS_CIRCLE,
        PS_SQUARE,
        PS_TRIANGLE,
        NINTENDO_A,
        NINTENDO_B,
        NINTENDO_X,
        NINTENDO_Y,
        START,
        SELECT,
        HOME,
        LTRIGGER,
        LTRIGGER_LIGHT,
        LTRIGGER_HARD,
        RTRIGGER,
        RTRIGGER_LIGHT,
        RTRIGGER_HARD,
        LBUMPER,
        RBUMPER,
        L3,
        R3
    }

    /// <summary>
    /// Game Input class. Here, we update our Controls object based on inputs.
    /// </summary>
    public class GameInput
    {
        /// <summary>
        /// HashMap that maps keyboard inputs to controls packets.
        /// </summary>
        public readonly Dictionary<int, List<KeyboardKey>> KeyboardControls = new();
        public readonly Dictionary<int, List<GamepadBool>> GamepadControls = new();

        //TODO implement Gamepad input

        /// <summary>
        /// Path to our controls file
        /// </summary>
        const string Path = "./controls.json";

        /// <summary>
        /// Player instance should initialize this. 
        /// It's bad code that could be easily remedied with an object-oriented approach. 
        /// Too bad; that's beyond the scope of this project for now. 
        /// </summary>
        public Controls Controls;

        /// <summary>
        /// Gamepad id
        /// </summary>
        public int Gamepad;

        public float Deadzone;

        /// <summary>
        /// Initializes the GameInput class. 
        /// Honestly I probably should've done an OOP approach lmao. 
        /// Anyhow, it creates controls.json with default controls if it doesn't already exist. 
        /// If controls.json already exists, it reads it.
        /// It then writes the crap to a thing and writes that thing to our hashmap.
        /// </summary>
        public GameInput(int gamepad = -1)
        {
            Gamepad = gamepad;
            Controls = new();
            ControlsMap keyboardControls;
            ControlsMap gamepadControls;
            ControlsContainer controlsContainer;
            if(!File.Exists(Path))
            {
                keyboardControls = new ControlsMap()
                {
                    up = new string[] { "W", "UP" },
                    down = new string[] { "S", "DOWN" },
                    left = new string[] { "A", "LEFT" },
                    right = new string[] { "D", "RIGHT" },
                    jump = new string[] { "SPACE" }
                };

                gamepadControls = new ControlsMap()
                {
                    up = new string[] { "LSTICK_UP", "DPAD_UP" },
                    down = new string[] { "LSTICK_DOWN", "DPAD_DOWN" },
                    left = new string[] { "LSTICK_LEFT", "DPAD_LEFT" },
                    right = new string[] { "LSTICK_RIGHT", "DPAD_RIGHT" },
                    jump = new string[] { "PS_X" }
                };

                controlsContainer = new ControlsContainer()
                {
                    keyboardControls = keyboardControls,
                    gamepadControls = gamepadControls,
                    deadzone = 0.07f
                };

                File.WriteAllTextAsync(Path, JsonSerializer.Serialize(controlsContainer));
            }
            else
            {
                controlsContainer = JsonSerializer.Deserialize<ControlsContainer>(File.ReadAllText(Path));
                keyboardControls = controlsContainer.keyboardControls;
                gamepadControls = controlsContainer.gamepadControls;
                Deadzone = controlsContainer.deadzone;
            }

            //mild reflection because fuck you that's why
            //don't worry; it's safe since we're just reflecting across enums and our json-style object
            foreach(ControlBits control in Enum.GetValues(typeof(ControlBits)))
            {
                string name = control.ToString();

                //keyboard
                string[] mappings = keyboardControls[name.ToLower()];
                List<KeyboardKey> keyMappings = new(mappings.Length);
                foreach(string mapping in mappings)
                {
                    keyMappings.Add((KeyboardKey)Enum.Parse(typeof(KeyboardKey), "KEY_" + mapping.ToUpper()));
                }
                KeyboardControls.Add((int)control, keyMappings);

                //gamepad
                mappings = gamepadControls[name.ToLower()];
                List<GamepadBool> gamepadMappings = new(mappings.Length);
                foreach (string mapping in mappings)
                {
                    gamepadMappings.Add((GamepadBool)Enum.Parse(typeof(GamepadBool), mapping.ToUpper()));
                }
                GamepadControls.Add((int)control, gamepadMappings);
            }
        }

        /// <summary>
        /// Ticks each frame. Updates the controls object.
        /// </summary>
        public void Tick()
        {
            Controls.Tick();


            foreach(int control in Enum.GetValues(typeof(ControlBits)))
            {
                bool keyB = false;
                bool gamepadB = false;

                //gamepad
                if(Gamepad >= 0)
                {
                    List<GamepadBool> inputs;
                    if(GamepadControls.TryGetValue(control, out inputs))
                    {
                        foreach(GamepadBool input in inputs)
                        {
                            gamepadB |= IsHeld(input);
                        }
                    }
                }

                //key
                {
                    List<KeyboardKey> inputs;
                    if (KeyboardControls.TryGetValue(control, out inputs))
                    {
                        foreach (KeyboardKey input in inputs)
                        {
                            keyB |= Raylib.IsKeyDown(input);
                        }
                    }
                }

                if(keyB || gamepadB)
                {
                    Controls.Press(control);
                }
                else
                {
                    Controls.Release(control);
                }
            }
        }

        public bool IsHeld(GamepadBool gamepadBool)
        {
            switch (gamepadBool)
            {
                case GamepadBool.LSTICK_UP:
                    return Raylib.GetGamepadAxisMovement(Gamepad, GamepadAxis.GAMEPAD_AXIS_LEFT_Y) < -Deadzone;
                case GamepadBool.LSTICK_DOWN:
                    return Raylib.GetGamepadAxisMovement(Gamepad, GamepadAxis.GAMEPAD_AXIS_LEFT_Y) > Deadzone;
                case GamepadBool.LSTICK_LEFT:
                    return Raylib.GetGamepadAxisMovement(Gamepad, GamepadAxis.GAMEPAD_AXIS_LEFT_X) < -Deadzone;
                case GamepadBool.LSTICK_RIGHT:
                    return Raylib.GetGamepadAxisMovement(Gamepad, GamepadAxis.GAMEPAD_AXIS_LEFT_X) > Deadzone;
                case GamepadBool.L3:
                    return Raylib.IsGamepadButtonDown(Gamepad, GamepadButton.GAMEPAD_BUTTON_LEFT_THUMB);
                case GamepadBool.RSTICK_UP:
                    return Raylib.GetGamepadAxisMovement(Gamepad, GamepadAxis.GAMEPAD_AXIS_LEFT_Y) < -Deadzone;
                case GamepadBool.RSTICK_DOWN:
                    return Raylib.GetGamepadAxisMovement(Gamepad, GamepadAxis.GAMEPAD_AXIS_LEFT_Y) > Deadzone;
                case GamepadBool.RSTICK_LEFT:
                    return Raylib.GetGamepadAxisMovement(Gamepad, GamepadAxis.GAMEPAD_AXIS_LEFT_X) < -Deadzone;
                case GamepadBool.RSTICK_RIGHT:
                    return Raylib.GetGamepadAxisMovement(Gamepad, GamepadAxis.GAMEPAD_AXIS_LEFT_X) > Deadzone;
                case GamepadBool.R3:
                    return Raylib.IsGamepadButtonDown(Gamepad, GamepadButton.GAMEPAD_BUTTON_RIGHT_THUMB);
                case GamepadBool.DPAD_UP:
                    return Raylib.IsGamepadButtonDown(Gamepad, GamepadButton.GAMEPAD_BUTTON_LEFT_FACE_UP);
                case GamepadBool.DPAD_DOWN:
                    return Raylib.IsGamepadButtonDown(Gamepad, GamepadButton.GAMEPAD_BUTTON_LEFT_FACE_DOWN);
                case GamepadBool.DPAD_LEFT:
                    return Raylib.IsGamepadButtonDown(Gamepad, GamepadButton.GAMEPAD_BUTTON_LEFT_FACE_LEFT);
                case GamepadBool.DPAD_RIGHT:
                    return Raylib.IsGamepadButtonDown(Gamepad, GamepadButton.GAMEPAD_BUTTON_LEFT_FACE_RIGHT);
                case GamepadBool.XBOX_A:
                case GamepadBool.PS_X:
                case GamepadBool.NINTENDO_B:
                    return Raylib.IsGamepadButtonDown(Gamepad, GamepadButton.GAMEPAD_BUTTON_RIGHT_FACE_DOWN);
                case GamepadBool.XBOX_B:
                case GamepadBool.PS_CIRCLE:
                case GamepadBool.NINTENDO_A:
                    return Raylib.IsGamepadButtonDown(Gamepad, GamepadButton.GAMEPAD_BUTTON_RIGHT_FACE_RIGHT);
                case GamepadBool.XBOX_X:
                case GamepadBool.PS_SQUARE:
                case GamepadBool.NINTENDO_Y:
                    return Raylib.IsGamepadButtonDown(Gamepad, GamepadButton.GAMEPAD_BUTTON_RIGHT_FACE_LEFT);
                case GamepadBool.XBOX_Y:
                case GamepadBool.PS_TRIANGLE:
                case GamepadBool.NINTENDO_X:
                    return Raylib.IsGamepadButtonDown(Gamepad, GamepadButton.GAMEPAD_BUTTON_RIGHT_FACE_UP);
                case GamepadBool.START:
                    return Raylib.IsGamepadButtonDown(Gamepad, GamepadButton.GAMEPAD_BUTTON_MIDDLE_RIGHT);
                case GamepadBool.SELECT:
                    return Raylib.IsGamepadButtonDown(Gamepad, GamepadButton.GAMEPAD_BUTTON_MIDDLE_LEFT);
                case GamepadBool.HOME:
                    return Raylib.IsGamepadButtonDown(Gamepad, GamepadButton.GAMEPAD_BUTTON_MIDDLE);
                case GamepadBool.LTRIGGER_HARD:
                    return Raylib.IsGamepadButtonDown(Gamepad, GamepadButton.GAMEPAD_BUTTON_LEFT_TRIGGER_2);
                case GamepadBool.LTRIGGER_LIGHT:
                    return Raylib.GetGamepadAxisMovement(Gamepad, GamepadAxis.GAMEPAD_AXIS_LEFT_TRIGGER) > -1;
                case GamepadBool.LTRIGGER:
                    return Raylib.IsGamepadButtonDown(Gamepad, GamepadButton.GAMEPAD_BUTTON_LEFT_TRIGGER_2) || 
                        Raylib.GetGamepadAxisMovement(Gamepad, GamepadAxis.GAMEPAD_AXIS_LEFT_TRIGGER) > -1;
                case GamepadBool.RTRIGGER_HARD:
                    return Raylib.IsGamepadButtonDown(Gamepad, GamepadButton.GAMEPAD_BUTTON_RIGHT_TRIGGER_2);
                case GamepadBool.RTRIGGER_LIGHT:
                    return Raylib.GetGamepadAxisMovement(Gamepad, GamepadAxis.GAMEPAD_AXIS_RIGHT_TRIGGER) > -1;
                case GamepadBool.RTRIGGER:
                    return Raylib.IsGamepadButtonDown(Gamepad, GamepadButton.GAMEPAD_BUTTON_RIGHT_TRIGGER_2) ||
                        Raylib.GetGamepadAxisMovement(Gamepad, GamepadAxis.GAMEPAD_AXIS_RIGHT_TRIGGER) > -1;
                case GamepadBool.LBUMPER:
                    return Raylib.IsGamepadButtonDown(Gamepad, GamepadButton.GAMEPAD_BUTTON_LEFT_TRIGGER_1);
                case GamepadBool.RBUMPER:
                    return Raylib.IsGamepadButtonDown(Gamepad, GamepadButton.GAMEPAD_BUTTON_RIGHT_TRIGGER_1);
            }

            return false;
        }
    }
}
