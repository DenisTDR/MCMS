// https://www.npmjs.com/package/deepmerge
// got dist/umd.js
// minified with https://javascript-minifier.com/
!function(e,r){"object"==typeof exports&&"undefined"!=typeof module?module.exports=r():"function"==typeof define&&define.amd?define(r):(e=e||self).deepmerge=r()}(this,function(){"use strict";var e=function(e){return function(e){return!!e&&"object"==typeof e}(e)&&!function(e){var t=Object.prototype.toString.call(e);return"[object RegExp]"===t||"[object Date]"===t||function(e){return e.$$typeof===r}(e)}(e)};var r="function"==typeof Symbol&&Symbol.for?Symbol.for("react.element"):60103;function t(e,r){return!1!==r.clone&&r.isMergeableObject(e)?i((t=e,Array.isArray(t)?[]:{}),e,r):e;var t}function n(e,r,n){return e.concat(r).map(function(e){return t(e,n)})}function o(e){return Object.keys(e).concat(function(e){return Object.getOwnPropertySymbols?Object.getOwnPropertySymbols(e).filter(function(r){return e.propertyIsEnumerable(r)}):[]}(e))}function c(e,r){try{return r in e}catch(e){return!1}}function u(e,r,n){var u={};return n.isMergeableObject(e)&&o(e).forEach(function(r){u[r]=t(e[r],n)}),o(r).forEach(function(o){(function(e,r){return c(e,r)&&!(Object.hasOwnProperty.call(e,r)&&Object.propertyIsEnumerable.call(e,r))})(e,o)||(c(e,o)&&n.isMergeableObject(r[o])?u[o]=function(e,r){if(!r.customMerge)return i;var t=r.customMerge(e);return"function"==typeof t?t:i}(o,n)(e[o],r[o],n):u[o]=t(r[o],n))}),u}function i(r,o,c){(c=c||{}).arrayMerge=c.arrayMerge||n,c.isMergeableObject=c.isMergeableObject||e,c.cloneUnlessOtherwiseSpecified=t;var i=Array.isArray(o);return i===Array.isArray(r)?i?c.arrayMerge(r,o,c):u(r,o,c):t(o,c)}return i.all=function(e,r){if(!Array.isArray(e))throw new Error("first argument should be an array");return e.reduce(function(e,t){return i(e,t,r)},{})},i});