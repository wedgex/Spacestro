using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spacestro.Framework
{
    public class Entity
    {
        public string Id { get; set; }

        private Dictionary<string, Component> components = new Dictionary<string, Component>();
        private Dictionary<string, Action> actions = new Dictionary<string, Action>();

        /// <summary>
        /// Adds the IComponent to the entity's IComponents dictionary using the IComponent's name as the key.
        /// </summary>
        /// <param name="IComponent">The IComponent to add.</param>
        public void AddIComponent(Component component)
        {
            this.components.Add(component.Name, component);
        }

        /// <summary>
        /// Remove the IComponent with the given name from the entity's IComponent dictionary.
        /// </summary>
        /// <param name="name">The name of the IComponent to remove.</param>
        public void RemoveIComponent(string name)
        {
            this.components.Remove(name);
        }

        /// <summary>
        /// Tries to find a IComponent with the given name and of type T.
        /// </summary>
        /// <typeparam name="T">The type of IComponent being searched for.</typeparam>
        /// <param name="name">Name of the IComponent to get.</param>
        /// <returns>The IComponent, null if no IComponent with that name was found or if it could not be cast to the desired type.</returns>
        public T GetIComponent<T>(string name) where T : Component
        {
            return (this.components.ContainsKey(name)) ? components[name] as T : null;
        }

        /// <summary>
        /// Add the IAction to the entity's IActions dictionary using the IAction's name as the key.
        /// </summary>
        /// <param name="action">The IAction to add.</param>
        public void AddIAction(Action action)
        {
            action.Entity = this;
            this.actions.Add(action.Name, action);
        }

        /// <summary>
        /// Remove the IAction with the given name from the entity's IAction dictionary.
        /// </summary>
        /// <param name="name">The name of the actoin to remove.</param>
        public void RemoveIAction(string name)
        {
            this.actions.Remove(name);
        }

        /// <summary>
        /// Attempt to call the IAction with the given name. If not is found, nothing happens.
        /// </summary>
        /// <param name="name">Name of the IAction to call.</param>
        public void DoIAction(string name)
        {
            if (this.actions.ContainsKey(name))
            {
                this.actions[name].Do();
            }
        }
    }
}
