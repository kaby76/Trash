using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;
using java.util;
using org.eclipse.wst.xml.xpath2.processor;
using org.w3c.dom;



namespace java.net
{
    public class URI
    {
        private string @string;

        public URI(string @string)
        {
            this.@string = @string;
        }

        public static URI create(string uri)
        {
            throw new NotImplementedException();
        }

        public bool Absolute { get; set; }

        public URI resolve(string uri)
        {
            throw new NotImplementedException();
        }
    }

    public class MalformedURLException : Exception
    {
    }

}

namespace java.text
{
    public class DecimalFormat
    {
        public DecimalFormat(String pattern, DecimalFormatSymbols symbols)
        {}

        public string toPattern()
        {
            return null;
        }

        public void applyPattern(String pattern) { }

        protected StringBuilder format(object o, StringBuilder stringBuilder, FieldPosition fieldPosition)
        {
            throw new NotImplementedException();
        }
    }

    public class DecimalFormatSymbols
    {
        private Locale uS;

        public DecimalFormatSymbols(Locale uS)
        {
            this.uS = uS;
        }
    }
    public class FieldPosition
    {
        private int v;

        public FieldPosition(int v)
        {
            this.v = v;
        }
    }
}

namespace java.time
{
    public class Duration { }
}

namespace java.util
{
    public class Locale
    {
        public static Locale US;
    }
    public class GregorianCalendar : Calendar
    {
        public static int BC;
        public static int AD;

        public GregorianCalendar(object getTimeZone)
        {
        }

        public GregorianCalendar()
        {
        }
    }

    public abstract class Calendar : IComparable
    {
        public static int MINUTE;
        public static int SECOND;
        public static int YEAR { get; internal set; }
        public static int ERA { get; set; }
        public static int MONTH { get; set; }
        public static int DAY_OF_MONTH { get; set; }
        public static int MILLISECOND { get; set; }
        public static int HOUR_OF_DAY { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }

        public Calendar clone()
        {
            throw new NotImplementedException();
        }

        public void AddHours(int hours)
        {
            throw new NotImplementedException();
        }

        public void AddMinutes(int minutes)
        {
            throw new NotImplementedException();
        }

        public int get(int year)
        {
            throw new NotImplementedException();
        }

        public long getTimeInMillis()
        {
            throw new NotImplementedException();
        }

        public void add(int month, int monthValue)
        {
            throw new NotImplementedException();
        }

        internal bool IsLessThan(Calendar thatcal)
        {
            throw new NotImplementedException();
        }

        internal bool IsGreaterThan(Calendar thatcal)
        {
            throw new NotImplementedException();
        }

        public static Calendar getInstance()
        {
            throw new NotImplementedException();
        }

        public void set(int year, int month, int day)
        {
            throw new NotImplementedException();
        }
        public void set(int field, int value)
        {
            throw new NotImplementedException();
        }

        public int getActualMinimum(int item)
        {
            throw new NotImplementedException();
        }

        public int getActualMaximum(int item)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }
    }

    public class TimeZone
    {
        private static TimeZone Instance;

        public static object getTimeZone(string gmt)
        {
            if (Instance == null)
            {
                Instance = new TimeZone();
            }

            return Instance;
        }

        public static TimeZone Default { get; set; }
    }
}

namespace java.xml
{
    public class XMLConstants
    {
        public static object XMLNS_ATTRIBUTE_NS_URI;
        public static object XMLNS_ATTRIBUTE;
        public static string NULL_NS_URI { get; set; }
        public static string DEFAULT_NS_PREFIX { get; set; }
    }
}

namespace javax.xml.datatype
{
    public class Duration
    {
        public int Days { get; internal set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }
        public int Sign { get; set; }

        public Duration negate()
        {
            throw new NotImplementedException();
        }
    }

    public class XMLGregorianCalendar
    {
        public void add(Duration negate)
        {
            throw new NotImplementedException();
        }

        public Calendar toGregorianCalendar()
        {
            throw new NotImplementedException();
        }
    }

    public class DatatypeFactory
    {
        public static DatatypeFactory newInstance()
        {
            return null;
        }

        public Duration newDuration(long duration)
        {
            throw new NotImplementedException();
        }

        public Duration newDuration(string duration)
        {
            throw new NotImplementedException();
        }

        public XMLGregorianCalendar newXMLGregorianCalendar(GregorianCalendar calendar)
        {
            throw new NotImplementedException();
        }

        public Duration newDuration(bool duration, int i, int i1, int i2, int hours, int minutes, int i3)
        {
            throw new NotImplementedException();
        }

        public XMLGregorianCalendar newXMLGregorianCalendarTime(int hour, int minute, int second, int i)
        {
            throw new NotImplementedException();
        }
    }

    public class DatatypeConfigurationException : Exception { }
}

namespace javax.xml.@namespace
{
    public class NamespaceContext
    {
        public virtual string getNamespaceURI(string prefix)
        {
            throw new NotImplementedException();
        }
    }

    public class QName
    {

        public QName(string namespaceURI, string localPart)
        {
            this.NamespaceURI = namespaceURI;
            this.LocalPart = localPart;
        }

        public QName(string namespaceURI, string localPart, string prefix)
        {
            this.NamespaceURI = namespaceURI;
            this.LocalPart = localPart;
            this.Prefix = prefix;
        }

        public string NamespaceURI { get; set; }
        public string LocalPart { get; set; }
        public string Prefix { get; set; }
    }

}

namespace javax.xml.parsers
{
    public abstract class DocumentBuilderFactor
    {
        public static DocumentBuilderFactor newInstance()
        {
            throw new NotImplementedException();
        }

        public bool NamespaceAware { get; set; }
        public bool Validating { get; set; }

        public DocumentBuilder newDocumentBuilder()
        {
            throw new NotImplementedException();
        }
    }

    public abstract class DocumentBuilder
    {
        public DOMBuilder.ErrorHandlerAnonymousInnerClass ErrorHandler { get; set; }

        public Document parse(Stream @in)
        {
            throw new NotImplementedException();
        }
    }
}

namespace org.apache.xerces.util
{
    public class XMLChar
    {
        public static bool isValidNCName(string strValue)
        {
            throw new NotImplementedException();
        }

        public static bool isValidName(string strValue)
        {
            throw new NotImplementedException();
        }

        public static bool isValidNmtoken(string strValue)
        {
            throw new NotImplementedException();
        }
    }
}

namespace org.apache.xerces.xs
{
    public interface XSModel
    {
    }
}

namespace org.apache.xerces.dom
{
    public class PSVIElementNSImpl
    {
        public bool Nil { get; set; }
    }
}

namespace org.w3c.dom
{
    
    public class NodeConstants
    {
        public const short TEXT_NODE = 1;
        public const short ELEMENT_NODE = 2;
        public const short DOCUMENT_NODE = 3;
        public const short COMMENT_NODE = 4;
        public const short ATTRIBUTE_NODE = 5;
        public const short CDATA_SECTION_NODE = 6;
        public const short PROCESSING_INSTRUCTION_NODE = 7;
        public const short DOCUMENT_POSITION_PRECEDING = 8;
        public const short DOCUMENT_POSITION_FOLLOWING = 9;
    }

    public interface Node
    {
        short NodeType { get; set; }
        string LocalName { get; set; }
        Document OwnerDocument { get; set; }
        NodeList ChildNodes { get; set; }
        Node NextSibling { get; set; }
        string BaseURI { get; set; }
        NamedNodeMap Attributes { get; set; }
        object NodeValue { get; set; }
        string NamespaceURI { get; set; }
        object NodeName { get; set; }
        Node ParentNode { get; set; }
        Node PreviousSibling { get; set; }
        bool isSameNode(Node nodeValue);
        short compareDocumentPosition(Node nodeB);
        bool isEqualNode(Node node);
        bool hasChildNodes();
        bool hasAttributes();
    }

    public interface CharacterData : Node
    {
        string Data { get; set; }
    }

    public interface Attr : Node
    {
        string Prefix { get; set; }
        object Name { get; set; }
        string Value { get; set; }
        Node OwnerElement { get; set; }
        TypeInfo SchemaTypeInfo { get; set; }
    }

    public interface Document : Node
    {
        string DocumentURI { get; set; }
        NodeList getElementsByTagNameNS(string ns, string local);
        bool isSupported(string core, string s);
    }

    public interface Element : Node
    {
        object getAttributeNS(string sCHEMA_INSTANCE, string nIL_ATTRIBUTE);
        string Prefix { get; set; }
        TypeInfo SchemaTypeInfo { get; set; }
        string lookupNamespaceURI(string prefix);
        bool isDefaultNamespace(object elementNamespaceUri);
    }

    public interface NamedNodeMap
    {
        int Length { get; set; }
        Attr item(int i);
    }
    public interface NodeList
    {
        int Length { get; set; }
        Node item(int i);
    }

    public interface Text : CharacterData
    {
    }

    public interface TypeInfo
    {
        string TypeName { get; set; }
    }

    public interface ProcessingInstruction : Node
    {
        string Data { get; set; }
        string Target { get; set; }
    }

    public interface Comment : CharacterData
    {

    }

}

namespace org.xml.sax
{
    public class SAXParseException : SAXException { }
    public class SAXException : Exception { }

    public class ParserConfigurationException : Exception { }
}
