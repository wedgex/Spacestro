using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spacestro.Framework
{
    public class Entity
    {
        Dictionary<string, Component> components = new Dictionary<string, Component>();
        Dictionary<string, Action> actions = new Dictionary<string, Action>();

        /// <summary>
        /// Adds the component to the entity's components dictionary using the component's name as the key.
        /// </summary>
        /// <param name="component">The component to add.</param>
        public void AddComponent(Component component)
        {
            this.components.Add(component.Name, component);
        }

        /// <summary>
        /// Remove the component with the given name from the entity's component dictionary.
        /// </summary>
        /// <param name="name">The name of the component to remove.</param>
        public void RemoveComponent(string name)
        {
            this.components.Remove(name);
        }

        /// <summary>
        /// Tries to find a component with the given name and of type T.
        /// </summary>
        /// <typeparam name="T">The type of component being searched for.</typeparam>
        /// <param name="name">Name of the component to get.</param>
        /// <returns>The component, null if no component with that name was found or if it could not be cast to the desired type.</returns>
        public T GetComponent<T>(string name) where T : Component
        {
            return (this.components.ContainsKey(name)) ? components[name] as T : null;
        }

        /// <summary>
        /// Add the action to the entity's actions dictionary using the action's name as the key.
        /// </summary>
        /// <param name="action">The action to add.</param>
        public void AddAction(Action action)
        {
            action.Entity = this;
            this.actions.Add(action.Name, action);
        }

        /// <summary>
        /// Remove the action with the given name from the entity's action dictionary.
        /// </summary>
        /// <param name="name">The name of the actoin to remove.</param>
        public void RemoveAction(string name)
        {
            this.actions.Remove(name);
        }

        /// <summary>
        /// Attempt to call the action with the given name. If not is found, nothing happens.
        /// </summary>
        /// <param name="name">Name of the action to call.</param>
        public void DoAction(string name)
        {
            if (this.actions.ContainsKey(name))
            {
                this.actions[name].Do();
            }
        }
    }
}
