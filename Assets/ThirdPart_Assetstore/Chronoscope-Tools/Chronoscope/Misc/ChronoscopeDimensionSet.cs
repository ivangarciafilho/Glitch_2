using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChronoscopeToolsInternal
{
    public class ChronoscopeDimensionSet
    {
        public float headerFooterHeight = 0;
        public int meterDivisions = 25;
        public int majorDivisionEvery = 5;
        public float meterDivisionWidth = 0;
        public float meterMajorLineTop = 0;
        public float meterMajorLineHeight = 0;
        public float meterMinorLineTop = 0;
        public float meterMinorLineHeight = 0;

        public float meterNormalizedLabelTop = 0;
        public float headerTextTop = 0;
        public float footerTextTop = 0;
        public float markerWidth = 0;

        public Rect graphicArea;
        public Rect meterArea;

        public Rect headerSeparator;
        public Rect footerSeparator;
        public Rect centreLine;

        public Rect eventMarker;

        public Rect leftBorder;
        public Rect rightBorder;
        public Rect topBorder;
        public Rect bottomBorder;

        public Rect centerMask;
        public Rect eventAreaTopLine;
        public Rect eventAreaBottomLine;
    } 
}
