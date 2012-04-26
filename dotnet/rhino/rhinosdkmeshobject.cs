#pragma warning disable 1591
using Rhino.Geometry;
using System;

#if RHINO_SDK
namespace Rhino.DocObjects
{
  public class MeshObject : RhinoObject 
  {
    internal MeshObject(uint serialNumber)
      : base(serialNumber) { }

    protected MeshObject() { }

    public Mesh MeshGeometry
    {
      get
      {
        Mesh rc = Geometry as Mesh;
        return rc;
      }
    }

    /// <summary>
    /// Only for developers who are defining custom subclasses of MeshObject.
    /// Directly sets the internal mesh geometry for this object.  Note that
    /// this function does not work with Rhino's "undo".
    /// </summary>
    /// <param name="mesh"></param>
    /// <returns>
    /// The old mesh geometry that was set for this object
    /// </returns>
    /// <remarks>
    /// Note that this function does not work with Rhino's "undo".  The typical
    /// approach for adjusting the mesh geometry is to modify the object that you
    /// get when you call the MeshGeometry property and then call CommitChanges.
    /// </remarks>
    protected Mesh SetMesh(Mesh mesh)
    {
      var parent = mesh.ParentRhinoObject();
      if (parent!=null && parent.RuntimeSerialNumber == this.RuntimeSerialNumber)
        return mesh;

      IntPtr pThis = this.NonConstPointer_I_KnowWhatImDoing();

      IntPtr pMesh = mesh.NonConstPointer();
      IntPtr pOldMesh = UnsafeNativeMethods.CRhinoMeshObject_SetMesh(pThis, pMesh);
      mesh.ChangeToConstObject(this);
      if (pOldMesh != pMesh && pOldMesh != IntPtr.Zero)
        return new Mesh(pOldMesh, null);
      return mesh;
    }

    public Mesh DuplicateMeshGeometry()
    {
      Mesh rc = DuplicateGeometry() as Mesh;
      return rc;
    }

    internal override CommitGeometryChangesFunc GetCommitFunc()
    {
      return UnsafeNativeMethods.CRhinoMeshObject_InternalCommitChanges;
    }
  }

  // skipping CRhinoMeshDensity, CRhinoObjectMesh, CRhinoMeshObjectsUI, CRhinoMeshStlUI
}

#endif