﻿using FasTnT.Formatters.Xml.Responses;
using FasTnT.Model;
using FasTnT.Model.Events.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace FasTnT.Formatters.Xml.Formatters.Events
{
    public class XmlAggregationEventFormatter
    {
        private XElement _root;
        private XElement _extension = new XElement("extension");

        public XElement Process(EpcisEvent aggregationEvent)
        {
            _root = XmlEventFormatter.CreateEvent("AggregationEvent", aggregationEvent);

            AddParentId(aggregationEvent);
            AddChildEpcs(aggregationEvent);
            AddAction(aggregationEvent);
            AddBusinessStep(aggregationEvent);
            AddDisposition(aggregationEvent);
            AddReadPoint(aggregationEvent);
            AddBusinessLocation(aggregationEvent);
            AddBusinessTransactions(aggregationEvent);
            AddSourceDestinations(aggregationEvent);
            AddEventExtension(aggregationEvent);
            AddExtensionField();
            AddCustomFields(aggregationEvent);

            return _root;
        }

        private void AddParentId(EpcisEvent evt)
        {
            var parentId = evt.Epcs.SingleOrDefault(e => e.Type == EpcType.ParentId);
            if (parentId != null) _root.Add(new XElement("parentID", parentId.Id));

        }

        private void AddCustomFields(EpcisEvent evt)
        {
            var customFields = CustomFields(evt, FieldType.CustomField);

            if (customFields.Any())
            {
                _root.Add(customFields);
            }
        }

        private void AddExtensionField()
        {
            if (_extension.HasElements || _extension.HasAttributes)
            {
                _root.Add(_extension);
            }
        }

        private void AddChildEpcs(EpcisEvent evt)
        {
            var childEpcs = evt.Epcs.Where(e => e.Type == EpcType.ChildEpc).Select(x => new XElement("epc", x.Id));
            var childQuantity = new XElement("childQuantityList", evt.Epcs.Where(x => x.Type == EpcType.ChildQuantity).Select(FormatQuantity));

            _root.Add(new XElement("childEPCs", childEpcs));
            if (childQuantity.HasElements) _extension.Add(childQuantity);
        }

        public XElement FormatQuantity(Epc epc)
        {
            var qtyElement = new XElement("quantityElement");
            qtyElement.Add(new XElement("epcClass", epc.Id));
            if (epc.Quantity.HasValue) qtyElement.Add(new XElement("quantity", epc.Quantity));
            if (!string.IsNullOrEmpty(epc.UnitOfMeasure)) qtyElement.Add(new XElement("uom", epc.UnitOfMeasure));

            return qtyElement;
        }

        private void AddAction(EpcisEvent objectEvent)
        {
            _root.Add(new XElement("action", objectEvent.Action.DisplayName));
        }

        private void AddBusinessStep(EpcisEvent evt)
        {
            if (!string.IsNullOrEmpty(evt.BusinessStep)) _root.Add(new XElement("bizStep", evt.BusinessStep));
        }

        public void AddDisposition(EpcisEvent evt)
        {
            if (!string.IsNullOrEmpty(evt.Disposition)) _root.Add(new XElement("disposition", evt.Disposition));
        }
        private void AddReadPoint(EpcisEvent evt)
        {
            if (string.IsNullOrEmpty(evt.ReadPoint)) return;

            var readPoint = new XElement("readPoint", new XElement("id", evt.ReadPoint));

            foreach (var ext in evt.CustomFields.Where(x => x.Type == FieldType.ReadPointExtension))
                readPoint.Add(new XElement(XName.Get(ext.Name, ext.Namespace), ext.TextValue));

            _root.Add(readPoint);
        }

        private void AddBusinessLocation(EpcisEvent evt)
        {
            if (string.IsNullOrEmpty(evt.BusinessLocation)) return;

            var custom = evt.CustomFields.Where(x => x.Type == FieldType.BusinessLocationExtension).Select(field => new XElement(XName.Get(field.Name, field.Namespace), field.TextValue));
            _root.Add(new XElement("bizLocation", new XElement("id", evt.BusinessLocation), custom));
        }

        private void AddBusinessTransactions(EpcisEvent evt)
        {
            if (evt.BusinessTransactions == null || !evt.BusinessTransactions.Any()) return;

            var transactions = new XElement("bizTransactionList");

            foreach (var trans in evt.BusinessTransactions)
                transactions.Add(new XElement("bizTransaction", trans.Id, new XAttribute("type", trans.Type)));

            _root.Add(transactions);
        }

        private void AddIlmdFields(EpcisEvent evt)
        {
            var ilmdElement = CustomFields(evt, FieldType.Ilmd);

            if (ilmdElement.Any())
            {
                _extension.Add(new XElement("ilmd", ilmdElement));
            }
        }

        private void AddSourceDestinations(EpcisEvent @event)
        {
            if (@event.SourceDestinationList == null || !@event.SourceDestinationList.Any()) return;

            var source = new XElement("sourceList");
            var destination = new XElement("destinationList");

            foreach (var sourceDest in @event.SourceDestinationList)
            {
                if (sourceDest.Direction == SourceDestinationType.Source)
                    source.Add(new XElement("source", new XAttribute("type", sourceDest.Type), sourceDest.Id));
                else if (sourceDest.Direction == SourceDestinationType.Destination)
                    destination.Add(new XElement("destination", new XAttribute("type", sourceDest.Type), sourceDest.Id));
            }

            if (source.HasElements)
            {
                _extension.Add(source);
            }
            if (destination.HasElements)
            {
                _extension.Add(destination);
            }
        }

        public void AddEventExtension(EpcisEvent evt)
        {
            var extensionElements = CustomFields(evt, FieldType.EventExtension);
            if (extensionElements.Any())
            {
                _extension.Add(new XElement("extension", extensionElements));
            }
        }

        private IEnumerable<XElement> CustomFields(EpcisEvent evt, FieldType type)
        {
            var elements = new List<XElement>();
            foreach (var rootField in evt.CustomFields.Where(x => x.Type == type))
            {
                var xmlElement = new XElement(XName.Get(rootField.Name, rootField.Namespace), rootField.TextValue);

                InnerCustomFields(xmlElement, type, rootField);
                foreach (var attribute in rootField.Children.Where(x => x.Type == FieldType.Attribute))
                {
                    xmlElement.Add(new XAttribute(XName.Get(attribute.Name, attribute.Namespace), attribute.TextValue));
                }

                elements.Add(xmlElement);
            }

            return elements;
        }

        private void InnerCustomFields(XContainer element, FieldType type, CustomField parent)
        {
            foreach (var field in parent.Children.Where(x => x.Type == type))
            {
                var xmlElement = new XElement(XName.Get(field.Name, field.Namespace), field.TextValue);

                InnerCustomFields(xmlElement, type, field);
                foreach (var attribute in field.Children.Where(x => x.Type == FieldType.Attribute))
                {
                    xmlElement.Add(new XAttribute(XName.Get(attribute.Name, attribute.Namespace), attribute.TextValue));
                }

                element.Add(xmlElement);
            }
        }
    }

}
