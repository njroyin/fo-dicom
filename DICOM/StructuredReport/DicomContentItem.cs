﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dicom.StructuredReport {
	public enum DicomValueType {
		Container,
		Text,
		Code,
		Numeric,
		PersonName,
		Date,
		Time,
		DateTime,
		UIDReference,
		Composite,
		Image,
		Waveform,
		SpatialCoordinate,
		TemporalCoordinate
	}

	public enum DicomContinuity {
		None,
		Separate,
		Continuous
	}

	public enum DicomRelationshipType {
		Contains,
		HasProperties,
		InferredFrom,
		SelectedFrom,
		HasObservationContext,
		HasAcquisitionContext,
		HasConceptModifier
	}

	public class DicomContentItem {
		private DicomDataset _dataset;

		public DicomContentItem(DicomDataset dataset) {
			Dataset = dataset;
		}

		public DicomContentItem(DicomCodeItem code, DicomRelationshipType relationship, DicomValueType type, string value) {
			Dataset = new DicomDataset();
			Code = code;
			Relationship = relationship;
			Type = type;

			if (type == DicomValueType.Text)
				Dataset.Add(DicomTag.TextValue, value);
			else if (type == DicomValueType.PersonName)
				Dataset.Add(DicomTag.PersonName, value);
			else
				throw new DicomStructuredReportException("Type of string is not the correct value type for {0} content item.", type);
		}

		public DicomContentItem(DicomCodeItem code, DicomRelationshipType relationship, DicomValueType type, DateTime value) {
			Dataset = new DicomDataset();
			Code = code;
			Relationship = relationship;
			Type = type;

			if (type == DicomValueType.Date)
				Dataset.Add(DicomTag.Date, value);
			else if (type == DicomValueType.Time)
				Dataset.Add(DicomTag.Time, value);
			else if (type == DicomValueType.DateTime)
				Dataset.Add(DicomTag.DateTime, value);
			else
				throw new DicomStructuredReportException("Type of DateTime is not the correct value type for {0} content item.", type);
		}

		public DicomContentItem(DicomCodeItem code, DicomRelationshipType relationship, DicomUID value) {
			Dataset = new DicomDataset();
			Code = code;
			Relationship = relationship;
			Type = DicomValueType.UIDReference;

			Dataset.Add(DicomTag.UID, value);
		}

		public DicomContentItem(DicomCodeItem code, DicomRelationshipType relationship, DicomCodeItem value) {
			Dataset = new DicomDataset();
			Code = code;
			Relationship = relationship;
			Type = DicomValueType.Code;

			Dataset.Add(DicomTag.ConceptCodeSequence, value);
		}

		public DicomContentItem(DicomCodeItem code, DicomRelationshipType relationship, DicomMeasuredValue value) {
			Dataset = new DicomDataset();
			Code = code;
			Relationship = relationship;
			Type = DicomValueType.Numeric;

			Dataset.Add(DicomTag.MeasuredValueSequence, value);
		}

		public DicomContentItem(DicomCodeItem code, DicomRelationshipType relationship, DicomValueType type, DicomReferencedSOP value) {
			Dataset = new DicomDataset();
			Code = code;
			Relationship = relationship;
			Type = type;

			if (type == DicomValueType.Composite || type == DicomValueType.Image || type == DicomValueType.Waveform)
				Dataset.Add(DicomTag.ReferencedSOPSequence, value);
			else
				throw new DicomStructuredReportException("Type of DicomReferencedSOP is not the correct value type for {0} content item.", type);
		}

		public DicomContentItem(DicomCodeItem code, DicomRelationshipType relationship, DicomContinuity continuity, params DicomContentItem[] items) {
			Dataset = new DicomDataset();
			Code = code;
			Relationship = relationship;
			Type = DicomValueType.Container;
			Continuity = continuity;

			Dataset.Add(DicomTag.ContentSequence, items);
		}

		public DicomDataset Dataset {
			get {
				return _dataset;
			}
			private set {
				_dataset = value;
			}
		}

		public DicomCodeItem Code {
			get {
				return Dataset.Get<DicomCodeItem>(DicomTag.ConceptNameCodeSequence);
			}
			private set {
				Dataset.Add(DicomTag.ConceptNameCodeSequence, value);
			}
		}

		public DicomValueType Type {
			get {
				var type = Dataset.Get<string>(DicomTag.ValueType, 0, "UNKNOWN");
				switch (type) {
				case "CONTAINER": return DicomValueType.Container;
				case "TEXT": return DicomValueType.Text;
				case "CODE": return DicomValueType.Code;
				case "NUM": return DicomValueType.Numeric;
				case "PNAME": return DicomValueType.PersonName;
				case "DATE": return DicomValueType.Date;
				case "TIME": return DicomValueType.Time;
				case "DATETIME": return DicomValueType.DateTime;
				case "UIDREF": return DicomValueType.UIDReference;
				case "COMPOSITE": return DicomValueType.Composite;
				case "IMAGE": return DicomValueType.Image;
				case "WAVEFORM": return DicomValueType.Waveform;
				case "SCOORD": return DicomValueType.SpatialCoordinate;
				case "TCOORD": return DicomValueType.TemporalCoordinate;
				default:
					throw new DicomStructuredReportException("Unknown value type: {0}", type);
				}
			}
			private set {
				switch (value) {
				case DicomValueType.Container: Dataset.Add(DicomTag.ValueType, "CONTAINER"); return;
				case DicomValueType.Text: Dataset.Add(DicomTag.ValueType, "TEXT"); return;
				case DicomValueType.Code: Dataset.Add(DicomTag.ValueType, "CODE"); return;
				case DicomValueType.Numeric: Dataset.Add(DicomTag.ValueType, "NUM"); return;
				case DicomValueType.PersonName: Dataset.Add(DicomTag.ValueType, "PNAME"); return;
				case DicomValueType.Date: Dataset.Add(DicomTag.ValueType, "DATE"); return;
				case DicomValueType.Time: Dataset.Add(DicomTag.ValueType, "TIME"); return;
				case DicomValueType.DateTime: Dataset.Add(DicomTag.ValueType, "DATETIME"); return;
				case DicomValueType.UIDReference: Dataset.Add(DicomTag.ValueType, "UIDREF"); return;
				case DicomValueType.Composite: Dataset.Add(DicomTag.ValueType, "COMPOSITE"); return;
				case DicomValueType.Image: Dataset.Add(DicomTag.ValueType, "IMAGE"); return;
				case DicomValueType.Waveform: Dataset.Add(DicomTag.ValueType, "WAVEFORM"); return;
				case DicomValueType.SpatialCoordinate: Dataset.Add(DicomTag.ValueType, "SCOORD"); return;
				case DicomValueType.TemporalCoordinate: Dataset.Add(DicomTag.ValueType, "TCOORD"); return;
				default: break;
				}
			}
		}

		public DicomRelationshipType Relationship {
			get {
				var type = Dataset.Get<string>(DicomTag.RelationshipType, 0, "UNKNOWN");
				switch (type) {
				case "CONTAINS": return DicomRelationshipType.Contains;
				case "HAS PROPERTIES": return DicomRelationshipType.HasProperties;
				case "INFERRED FROM": return DicomRelationshipType.InferredFrom;
				case "SELECTED FROM": return DicomRelationshipType.SelectedFrom;
				case "HAS OBS CONTEXT": return DicomRelationshipType.HasObservationContext;
				case "HAS ACQ CONTEXT": return DicomRelationshipType.HasAcquisitionContext;
				case "HAS CONCEPT MOD": return DicomRelationshipType.HasConceptModifier;
				default:
					throw new DicomStructuredReportException("Unknown relationship type: {0}", type);
				}
			}
			private set {
				switch (value) {
				case DicomRelationshipType.Contains: Dataset.Add(DicomTag.RelationshipType, "CONTAINS"); return;
				case DicomRelationshipType.HasProperties: Dataset.Add(DicomTag.RelationshipType, "HAS PROPERTIES"); return;
				case DicomRelationshipType.InferredFrom: Dataset.Add(DicomTag.RelationshipType, "INFERRED FROM"); return;
				case DicomRelationshipType.SelectedFrom: Dataset.Add(DicomTag.RelationshipType, "SELECTED FROM"); return;
				case DicomRelationshipType.HasObservationContext: Dataset.Add(DicomTag.RelationshipType, "HAS OBS CONTEXT"); return;
				case DicomRelationshipType.HasAcquisitionContext: Dataset.Add(DicomTag.RelationshipType, "HAS ACQ CONTEXT"); return;
				case DicomRelationshipType.HasConceptModifier: Dataset.Add(DicomTag.RelationshipType, "HAS CONCEPT MOD"); return;
				default: break;
				}
			}
		}

		public DicomContinuity Continuity {
			get {
				return Dataset.Get<DicomContinuity>(DicomTag.ContinuityOfContent, 0, DicomContinuity.None);
			}
			private set {
				Dataset.Add(DicomTag.ContinuityOfContent, value.ToString().ToUpper());
			}
		}

		public IEnumerable<DicomContentItem> Children() {
			var sequence = Dataset.Get<DicomSequence>(DicomTag.ContentSequence);

			// silence exceptions for items without a content sequence
			if (sequence == null)
				sequence = new DicomSequence(DicomTag.ContentSequence);

			foreach (var item in sequence)
				yield return new DicomContentItem(item);
		}

		public DicomContentItem Add(DicomContentItem item) {
			var sequence = Dataset.Get<DicomSequence>(DicomTag.ContentSequence);

			if (sequence == null) {
				sequence = new DicomSequence(DicomTag.ContentSequence);
				Dataset.Add(sequence);
			}

			sequence.Items.Add(item.Dataset);

			return item;
		}

		public DicomContentItem Add(DicomCodeItem code, DicomRelationshipType relationship, DicomValueType type, string value) {
			return Add(new DicomContentItem(code, relationship, type, value));
		}

		public DicomContentItem Add(DicomCodeItem code, DicomRelationshipType relationship, DicomValueType type, DateTime value) {
			return Add(new DicomContentItem(code, relationship, type, value));
		}

		public DicomContentItem Add(DicomCodeItem code, DicomRelationshipType relationship, DicomUID value) {
			return Add(new DicomContentItem(code, relationship, value));
		}

		public DicomContentItem Add(DicomCodeItem code, DicomRelationshipType relationship, DicomCodeItem value) {
			return Add(new DicomContentItem(code, relationship, value));
		}

		public DicomContentItem Add(DicomCodeItem code, DicomRelationshipType relationship, DicomMeasuredValue value) {
			return Add(new DicomContentItem(code, relationship, value));
		}

		public DicomContentItem Add(DicomCodeItem code, DicomRelationshipType relationship, DicomValueType type, DicomReferencedSOP value) {
			return Add(new DicomContentItem(code, relationship, type, value));
		}

		public DicomContentItem Add(DicomCodeItem code, DicomRelationshipType relationship, DicomContinuity continuity, params DicomContentItem[] items) {
			return Add(new DicomContentItem(code, relationship, continuity, items));
		}

		public T Get<T>() {
			if (typeof(T) == typeof(string)) {
				if (Type == DicomValueType.Text)
					return (T)(object)Dataset.Get<string>(DicomTag.TextValue, 0, String.Empty);
				if (Type == DicomValueType.PersonName)
					return (T)(object)Dataset.Get<string>(DicomTag.PersonName, 0, String.Empty);
			}

			if (typeof(T) == typeof(DateTime)) {
				if (Type == DicomValueType.Date)
					return (T)(object)Dataset.Get<DateTime>(DicomTag.Date, 0, DateTime.Today);
				if (Type == DicomValueType.Time)
					return (T)(object)Dataset.Get<DateTime>(DicomTag.Time, 0, DateTime.Today);
				if (Type == DicomValueType.DateTime)
					return (T)(object)Dataset.Get<DateTime>(DicomTag.DateTime, 0, DateTime.Today);
			}

			if (typeof(T) == typeof(DicomUID)) {
				if (Type == DicomValueType.UIDReference)
					return (T)(object)Dataset.Get<DicomUID>(DicomTag.UID);
			}

			if (typeof(T) == typeof(DicomCodeItem)) {
				if (Type == DicomValueType.Code)
					return (T)(object)Dataset.Get<DicomCodeItem>(DicomTag.ConceptCodeSequence);
			}

			if (typeof(T) == typeof(DicomMeasuredValue)) {
				if (Type == DicomValueType.Numeric)
					return (T)(object)Dataset.Get<DicomMeasuredValue>(DicomTag.MeasuredValueSequence);
			}

			if (typeof(T) == typeof(DicomReferencedSOP)) {
				if (Type == DicomValueType.Composite || Type == DicomValueType.Image || Type == DicomValueType.Waveform)
					return (T)(object)Dataset.Get<DicomReferencedSOP>(DicomTag.ReferencedSOPSequence);
			}

			throw new DicomStructuredReportException("Unable to get type of {0} from {1} content item.", typeof(T), Type);
		}
	}
}
