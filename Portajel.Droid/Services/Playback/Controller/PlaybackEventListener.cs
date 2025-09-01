using AndroidX.Media3.Common;
using Java.Interop;

namespace Portajel.Droid.Playback;

public class PlaybackEventListener: Object, IPlayerListener
{
    public IntPtr Handle { get; }
    public int JniIdentityHashCode { get; set; }
    public JniObjectReference PeerReference { get; set; }
    public JniPeerMembers JniPeerMembers { get; set; }
    public JniManagedPeerStates JniManagedPeerState { get; set; }
    
    public void Dispose()
    {
        
    }
    
    public void SetJniIdentityHashCode(int value)
    {
        JniIdentityHashCode = value;
    }
    
    public void SetPeerReference(JniObjectReference reference)
    {
        PeerReference = reference;
    }
    
    public void SetJniManagedPeerState(JniManagedPeerStates value)
    {
        JniManagedPeerState = value;
    }
    
    public void UnregisterFromRuntime()
    {
       
    }
    
    public void DisposeUnlessReferenced()
    {
        
    }
    
    public void Disposed()
    {
        
    }
    
    public void Finalized()
    {
        
    }

}