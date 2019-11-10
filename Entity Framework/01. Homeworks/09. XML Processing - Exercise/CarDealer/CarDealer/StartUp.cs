﻿namespace CarDealer
{
    using System;
    using System.Text;
    using System.Linq;
    using CarDealer.Dtos.Import;
    using System.IO;
    using CarDealer.Data;
    using System.Collections.Generic;
    using CarDealer.Models;
    using AutoMapper;
    using System.Xml.Serialization;

    public class StartUp
    {
        public static void Main()
        {
            Mapper.Initialize(x =>
            {
                x.AddProfile<CarDealerProfile>();
            });

            var context = new CarDealerContext();

            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();

            //var suppliers = File.ReadAllText(@"D:\Documents\GitHub\Software-University\Entity Framework\01. Homeworks\09. XML Processing - Exercise\CarDealer\CarDealer\Datasets\suppliers.xml");

            var parts = File.ReadAllText(@"D:\Documents\GitHub\Software-University\Entity Framework\01. Homeworks\09. XML Processing - Exercise\CarDealer\CarDealer\Datasets\parts.xml");

            //Console.WriteLine(ImportSuppliers(context, suppliers));
            Console.WriteLine(ImportParts(context, parts));
        }

        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            var xmlSerializer =
                new XmlSerializer(typeof(ImportSupplierDto[]), new XmlRootAttribute("Suppliers"));
            var suppliersDto = (ImportSupplierDto[])xmlSerializer.Deserialize(new StringReader(inputXml));

            List<Supplier> suppliers = new List<Supplier>();

            foreach (var supplierDto in suppliersDto)
            {
                var supplier = Mapper.Map<Supplier>(supplierDto);
                suppliers.Add(supplier);
            }

            context.Suppliers.AddRange(suppliers);
            int count = context.SaveChanges();

            return $"Successfully imported {count}";
        }

        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportPartDto[]), new XmlRootAttribute("Parts"));
            var partsDto = (ImportPartDto[])xmlSerializer.Deserialize(new StringReader(inputXml));

            List<Part> parts = new List<Part>();

            foreach (var partDto in partsDto)
            {
                var supplier = context.Suppliers.Find(partDto.SupplierId);

                if (supplier != null)
                {
                    var part = Mapper.Map<Part>(partDto);
                    parts.Add(part);
                }
            }

            context.Parts.AddRange(parts);
            int count = context.SaveChanges();

            return $"Successfully imported {count}";
        }
    }
}