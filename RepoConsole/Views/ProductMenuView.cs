﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepoConsole.Presenter;

namespace RepoConsole.Views
{
    class ProductMenuView : IViewProduct
    {
        public event EventHandler<EventArgs> GetAll;
        public event EventHandler<EventArgs> Remove;
        public event EventHandler<EventArgs> Add;
        public event EventHandler<EventArgs> Get;
        public int Id { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }
        private Presenter.IPresenter _presenter;
        private bool _exit = false;

        public ProductMenuView()
        {
            _presenter = new ProductPresenter(this);
        }

        public void WaitForInput()
        {
            var input = Console.ReadLine();
            int result;

            if (int.TryParse(input, out result))
            {

                switch (result)
                {
                    case 1:
                        Show_Add();
                        break;
                    case 2:
                        Show_Get();
                        break;
                    case 3:
                        Show_GetAll();
                        break;
                    case 4:
                        Show_Remove();
                        break;
                    case 5:
                        _exit = true;
                        break;
                }

            }
        }

        public void Show_Add()
        {
            Console.Clear();
            Console.WriteLine("Add New Product\n");
            Console.Write("Name: ");
            Name = Console.ReadLine();
            Console.Write("Price: ");

            var input = Console.ReadLine();
            float result;

            if (float.TryParse(input, out result))
            {
                Price = result;
            }

            if (Add != null) Add(this, EventArgs.Empty);

            Console.ReadLine();
        }

        public void Show_Get()
        {
            Console.Clear();
            Console.WriteLine("Get Employee\n");
            Console.Write("ID: ");

            var input = Console.ReadLine();
            int result;

            if (int.TryParse(input, out result))
            {
                Id = result;
                Get(this, EventArgs.Empty);
            }

            Console.Read();
        }

        public void Show_GetAll()
        {
            Console.Clear();
            Console.WriteLine("List all employees\n");
            GetAll(this, EventArgs.Empty);
            Console.ReadLine();
        }

        public void Show_Remove()
        {
            Console.Clear();
            Console.WriteLine("Remove Employee\n");
            Console.Write("ID: ");

            var input = Console.ReadLine();
            int result;

            if (int.TryParse(input, out result))
            {
                Id = result;
                Remove(this, EventArgs.Empty);
            }

            Console.Read();
        }

        public void Show()
        {
            do
            {
                Console.Clear();
                Console.WriteLine("Products Menu\n");
                Console.WriteLine("1: Add new product");
                Console.WriteLine("2: Get product");
                Console.WriteLine("3: Get All");
                Console.WriteLine("4: Remove product");
                Console.WriteLine("5: Back");
                Console.Write("\nChoice: ");
                WaitForInput();
            } while (_exit == false);
        }
    }
}