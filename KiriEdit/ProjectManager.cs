using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiriEdit.Model
{
    internal class ProjectManager
    {
        public CharacterCollection Characters { get; }

        public Project Project { get; private set; }

        public bool IsProjectOpen { get => Project != null; }

        public void CloseProject()
        {
            throw new NotImplementedException();
        }

#if false
        public void MakeProject(string familyName, FontStyle fontStyle)
        {
            if (IsProjectOpen)
                throw new ApplicationException();

            var project = new Project();

            project.FontFamilyName = familyName;
            project.FontStyle = fontStyle;

            ValidateProject(project);

            Project = project;
        }
#endif

        public void OpenProject(string projectPath)
        {
            Project project = null;

            // TODO: load

            ValidateProject(project);
        }

        public void SaveProject(string projectPath)
        {
            throw new NotImplementedException();
        }

        private void ValidateProject(Project project)
        {
            throw new NotImplementedException();
        }


        #region Inner classes

        //--------------------------------------------------------------------

        public class CharacterCollection : ItemCollection<uint, Character>
        {
            public void Add(Character character)
            {
                Add(character.CodePoint, character);
            }

            public void Remove(Character character)
            {
                Remove(character.CodePoint);
            }
        }

        public class ItemCollection<TKey, TValue> : INotifyCollectionChanged, IEnumerable<TValue>
        {
            private Dictionary<TKey, TValue> _map = new Dictionary<TKey, TValue>();

            public event NotifyCollectionChangedEventHandler CollectionChanged;

            public int Count { get => _map.Count; }

            public void Add(TKey key, TValue value)
            {
                _map.Add(key, value);
                OnCollectionChanged(NotifyCollectionChangedAction.Add);
            }

            public void Remove(TKey key)
            {
                _map.Remove(key);
                OnCollectionChanged(NotifyCollectionChangedAction.Remove);
            }

            public bool Contains(TKey key)
            {
                return _map.ContainsKey(key);
            }

            public IEnumerator<TValue> GetEnumerator()
            {
                return _map.Values.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return _map.Values.GetEnumerator();
            }

            private void OnCollectionChanged(NotifyCollectionChangedAction action)
            {
                if (CollectionChanged != null)
                {
                    var e = new NotifyCollectionChangedEventArgs(action);
                    CollectionChanged(this, e);
                }
            }
        }

        #endregion
    }
}
