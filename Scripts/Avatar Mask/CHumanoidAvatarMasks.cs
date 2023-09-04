using System;
using System.Linq;
using UnityEngine;

namespace CDK.AvatarMask {
    public static class CHumanoidAvatarMasks {

        public static void SetActiveBodyParts(this UnityEngine.AvatarMask avatarMask, params AvatarMaskBodyPart[] parts) {
            var allEnums = Enum.GetValues(typeof(AvatarMaskBodyPart)).Cast<AvatarMaskBodyPart>();
            foreach (var bp in allEnums) {
                try {
                    avatarMask.SetHumanoidBodyPartActive(bp, parts.Contains(bp));
                }
                catch (Exception ex) {
                    Debug.LogError("Body Part: " + bp + " - " + ex.Message);
                }
            }
        }
        
        public static UnityEngine.AvatarMask UpperBody {
            get {
                if(_upperBody != null) return _upperBody;
                var avatar = new UnityEngine.AvatarMask {
                    name = "Upper Body",
                };
                avatar.SetActiveBodyParts(
                    AvatarMaskBodyPart.Body,
                    AvatarMaskBodyPart.Head,
                    // AvatarMaskBodyPart.LeftLeg,
                    // AvatarMaskBodyPart.RightLeg,
                    AvatarMaskBodyPart.LeftArm,
                    AvatarMaskBodyPart.RightArm,
                    AvatarMaskBodyPart.LeftFingers,
                    AvatarMaskBodyPart.RightFingers,
                    // AvatarMaskBodyPart.LeftFootIK,
                    // AvatarMaskBodyPart.RightFootIK,
                    AvatarMaskBodyPart.LeftHandIK,
                    AvatarMaskBodyPart.RightHandIK    
                );
                return _upperBody = avatar;
            }
        }
        private static UnityEngine.AvatarMask _upperBody;
        
        public static UnityEngine.AvatarMask Arms {
            get {
                if(_arms != null) return _arms;
                var avatar = new UnityEngine.AvatarMask {
                    name = "Arms",
                };
                avatar.SetActiveBodyParts(
                    // AvatarMaskBodyPart.Body,
                    // AvatarMaskBodyPart.Head,
                    // AvatarMaskBodyPart.LeftLeg,
                    // AvatarMaskBodyPart.RightLeg,
                    AvatarMaskBodyPart.LeftArm,
                    AvatarMaskBodyPart.RightArm,
                    AvatarMaskBodyPart.LeftFingers,
                    AvatarMaskBodyPart.RightFingers,
                    // AvatarMaskBodyPart.LeftFootIK,
                    // AvatarMaskBodyPart.RightFootIK,
                    AvatarMaskBodyPart.LeftHandIK,
                    AvatarMaskBodyPart.RightHandIK    
                );
                return _arms = avatar;
            }
        }
        private static UnityEngine.AvatarMask _arms;

        public static UnityEngine.AvatarMask Head {
            get {
                if(_head != null) return _head;
                var avatar = new UnityEngine.AvatarMask {
                    name = "Head",
                };
                avatar.SetActiveBodyParts(
                    AvatarMaskBodyPart.Head
                );
                return _head = avatar;
            }
        }
        private static UnityEngine.AvatarMask _head;

        public static UnityEngine.AvatarMask Legs {
            get {
                if(_legs != null) return _legs;
                var avatar = new UnityEngine.AvatarMask {
                    name = "Legs",
                };
                avatar.SetActiveBodyParts(
                    // AvatarMaskBodyPart.Body,
                    // AvatarMaskBodyPart.Head,
                    AvatarMaskBodyPart.LeftLeg,
                    AvatarMaskBodyPart.RightLeg,
                    // AvatarMaskBodyPart.LeftArm,
                    // AvatarMaskBodyPart.RightArm,
                    // AvatarMaskBodyPart.LeftFingers,
                    // AvatarMaskBodyPart.RightFingers,
                    AvatarMaskBodyPart.LeftFootIK,
                    AvatarMaskBodyPart.RightFootIK
                    // AvatarMaskBodyPart.LeftHandIK,
                    // AvatarMaskBodyPart.RightHandIK    
                );
                return _legs = avatar;
            }
        }   
        private static UnityEngine.AvatarMask _legs;
        
        public static UnityEngine.AvatarMask LowerBody {
            get {
                if(_lowerBody != null) return _lowerBody;
                var avatar = new UnityEngine.AvatarMask {
                    name = "Lower Body",
                };
                avatar.SetActiveBodyParts(
                    AvatarMaskBodyPart.Body,
                    // AvatarMaskBodyPart.Head,
                    AvatarMaskBodyPart.LeftLeg,
                    AvatarMaskBodyPart.RightLeg,
                    // AvatarMaskBodyPart.LeftArm,
                    // AvatarMaskBodyPart.RightArm,
                    // AvatarMaskBodyPart.LeftFingers,
                    // AvatarMaskBodyPart.RightFingers,
                    AvatarMaskBodyPart.LeftFootIK,
                    AvatarMaskBodyPart.RightFootIK
                    // AvatarMaskBodyPart.LeftHandIK,
                    // AvatarMaskBodyPart.RightHandIK    
                );
                return _lowerBody = avatar;
            }
        }
        private static UnityEngine.AvatarMask _lowerBody;
        
        public static UnityEngine.AvatarMask LeftArm {
            get {
                if(_leftArm != null) return _leftArm;
                var avatar = new UnityEngine.AvatarMask {
                    name = "Left Arm",
                };
                avatar.SetActiveBodyParts(
                    AvatarMaskBodyPart.LeftArm,
                    AvatarMaskBodyPart.LeftFingers,
                    AvatarMaskBodyPart.LeftHandIK
                );
                return _leftArm = avatar;
            }
        }
        private static UnityEngine.AvatarMask _leftArm;
        
        public static UnityEngine.AvatarMask RightArm {
            get {
                if(_rightArm != null) return _rightArm;
                var avatar = new UnityEngine.AvatarMask {
                    name = "Right Arm",
                };
                avatar.SetActiveBodyParts(
                    AvatarMaskBodyPart.RightArm,
                    AvatarMaskBodyPart.RightFingers,
                    AvatarMaskBodyPart.RightHandIK    
                );
                return _rightArm = avatar;
            }
        }
        private static UnityEngine.AvatarMask _rightArm;
        
        public static UnityEngine.AvatarMask LeftLeg {
            get {
                if(_leftLeg != null) return _leftLeg;
                var avatar = new UnityEngine.AvatarMask {
                    name = "Left Leg",
                };
                avatar.SetActiveBodyParts(
                    AvatarMaskBodyPart.LeftLeg,
                    AvatarMaskBodyPart.LeftFootIK
                );
                return _leftLeg = avatar;
            }
        }
        private static UnityEngine.AvatarMask _leftLeg;
        
        public static UnityEngine.AvatarMask RightLeg {
            get {
                if(_rightLeg != null) return _rightLeg;
                var avatar = new UnityEngine.AvatarMask {
                    name = "Right Leg",
                };
                avatar.SetActiveBodyParts(
                    AvatarMaskBodyPart.RightLeg,
                    AvatarMaskBodyPart.RightFootIK
                );
                return _rightLeg = avatar;
            }
        }
        private static UnityEngine.AvatarMask _rightLeg;
        
    }
}