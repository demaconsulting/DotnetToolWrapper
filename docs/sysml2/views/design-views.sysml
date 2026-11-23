// Views rendered by SysML2Tools into docs/design/generated/ (one SVG per view, named after the
// view) and embedded in the Software Design Document. `expose` scopes what each diagram shows;
// the view definition or `render` member only selects the drawing style.
//
// The OTS items in model/ots.sysml are build, test, and documentation tools rather than parts of
// the shipped wrapper, so no view draws them; the OTS integration design lists them instead.
package DotnetToolWrapperDesignViews {
    private import StandardViewDefinitions::*;
    private import Views::*;
    private import DotnetToolWrapper::*;

    view SoftwareStructureView : InterconnectionView {
        expose DotnetToolWrapper::DotnetToolWrapperSystem;
        filter not (@SysML::Documentation);
    }

    view DotnetToolWrapperView {
        expose DotnetToolWrapperSystem;
        expose DotnetToolWrapperSystem::*;
        render asInterconnectionDiagram;
    }
}
