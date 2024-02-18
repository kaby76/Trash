namespace Workspaces
{
    using System;
    using System.Collections.Generic;

    public sealed class DocumentId
    {
        public Guid Id { get; }

        internal string DebugName { get; }

        private DocumentId(Guid id, string debugName)
        {
            Id = id;
            DebugName = debugName;
        }

        public static DocumentId CreateNewId(string debugName = null)
        {
            return new DocumentId(Guid.NewGuid(), debugName);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as DocumentId);
        }

        public bool Equals(DocumentId other)
        {
            return other is object && Id == other.Id;
        }

        internal string GetDebuggerDisplay()
        {
            return string.Format("({0}, #{1} - {2})", GetType().Name, Id, DebugName);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return GetDebuggerDisplay();
        }

        public static bool operator ==(DocumentId left, DocumentId right)
        {
            return EqualityComparer<DocumentId>.Default.Equals(left, right);
        }

        public static bool operator !=(DocumentId left, DocumentId right)
        {
            return !(left == right);
        }
    }
}
