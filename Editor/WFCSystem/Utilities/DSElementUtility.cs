using DS.Elements;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System;

namespace DS.Utilities 
{
    public static class DSElementUtility
    {
        public static TextField CreateTextField(string value = null, EventCallback<ChangeEvent<string>> onValueChange = null)
        {
            TextField textField = new TextField() 
            {
                value = value
            };


            if (onValueChange != null) 
            {
            textField.RegisterValueChangedCallback(onValueChange);
            }

            return textField;

        }

        public static int GetPortIdx(string portName) 
        {

            switch (portName)
            {

                case "Left Side":
                    return 0;
                case "Up Side":
                    return 1;
                case "Right Side":
                    return 2;
                case "Down Side":
                    return 3;

                default:
                    return -1;
            }




        }


        public static TextField CreateTextArea(string value = null, EventCallback<ChangeEvent<string>> onValueChange = null) 
        {
            TextField textArea = CreateTextField(value, onValueChange);

            textArea.multiline = true;

            return textArea;
        }


        public static Foldout CreateFoldout(string title ,bool collapsed = false)
        {
            Foldout foldout = new Foldout()
            {
                text = title,
                value = collapsed
            };


            return foldout;
        }


        public static Button CreateButton(string text, Action onClick = null)
        {
            Button button = new Button(onClick)
            {
                text = text
            };

            return button;
        }


        public static Port CreatePort(this DSNode node, string portName = "", Orientation orientation = Orientation.Horizontal, Direction direction = Direction.Output, Port.Capacity capacity = Port.Capacity.Single)
        {
            Port port = node.InstantiatePort(orientation, direction, capacity, typeof(bool));

            port.portName = portName;

            return port;
        }


    }

}
