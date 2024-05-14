using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekat
{
    public class Cache
    {
        private int capacity, size;
        private ListNode head, tail;
        private Dictionary<string, ListNode> dictionary;

        class ListNode 
        {
            public string key;
            public byte[] val;
            public ListNode prev, next;
            public ListNode(string k, byte[] v)
            {
                key = k;
                val = v;
                prev = null;
                next = null;
            }
        }

        public Cache(int capacity)
        {
            if (capacity <= 0)
            {
                throw new ArgumentException("Capacity must be greater than zero.");
            }

            this.capacity = capacity;
            size = 0;
            head = new ListNode(null, null);
            tail = new ListNode(null, null);
            tail.prev = head;
            head.next = tail;
            dictionary = new Dictionary<string, ListNode>();
        }

        public bool ContainsKey(string key)
        {
            return dictionary.ContainsKey(key);
        }

        public byte[] Get(string key)
        {
            if (dictionary.TryGetValue(key, out ListNode target))
            {
                Remove(target);
                AddToLast(target);
                return target.val!;
            }
            return null;
        }

        public void Set(string key, byte[] value)
        {
            if (dictionary.TryGetValue(key, out ListNode target))
            {
                target.val = value;
                Remove(target);
                AddToLast(target);
            }
            else
            {
                if (size == capacity)
                {
                    dictionary.Remove(head.next.key);
                    Remove(head.next);
                    --size;
                }

                ListNode newNode = new ListNode(key, value);
                dictionary.Add(key, newNode);
                AddToLast(newNode);
                ++size;
            }
        }


        private void AddToLast(ListNode target)
        {
            target.next = tail;
            target.prev = tail.prev;
            tail.prev.next = target;
            tail.prev = target;
        }

        private void Remove(ListNode target)
        {
            target.next.prev = target.prev;
            target.prev.next = target.next;
        }
    }
}

