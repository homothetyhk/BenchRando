using Benchwarp;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Components;
using ItemChanger.Util;

namespace BenchRando.IC
{
    public class BenchContainer : Container
    {
        public override string Name => Bench;
        public const string Bench = "Bench";

        public override bool SupportsInstantiate => true;

        public override GameObject GetNewContainer(AbstractPlacement placement, IEnumerable<AbstractItem> items, FlingType flingType, Cost cost = null, Transition? changeSceneTo = null)
        {
            if (!ObjectCache.DidPreload) throw new InvalidOperationException("Cannot create bench container; Benchwarp did not preload.");
            GameObject bench = ObjectCache.GetNewBench();
            if (placement.GetPlacementAndLocationTags().OfType<BenchStyleTag>().FirstOrDefault() is BenchStyleTag bst)
            {
                BenchMaker.ApplyStyleSprites(bench, bst.FarStyle, bst.NearStyle);
            }
            return bench;
        }


        public override void AddGiveEffectToFsm(PlayMakerFSM fsm, ContainerGiveInfo info)
        {
            if (fsm.FsmName != "Bench Control") return;

            if (info.placement.GetPlacementAndLocationTags().OfType<RequireUnlockToRestTag>().Any()
                && !Benchwarp.Benchwarp.LS.visitedBenchScenes.Contains(new(fsm.gameObject.scene.name, fsm.gameObject.name)))
            {
                AddGiveEffectAndPreventRest(fsm, info);
                AddGiveEffectAfterRest(fsm, info);
            }
            else
            {
                AddGiveEffectAfterRest(fsm, info);
            }
        }

        public static void AddGiveEffectAndPreventRest(PlayMakerFSM fsm, ContainerGiveInfo info)
        {
            FsmGameObject prompt = fsm.FsmVariables.FindFsmGameObject("Prompt");
            FsmGameObject arrow = new(fsm.GetState("In Range").GetFirstActionOfType<ShowPromptMarker>().prefab);
            FsmGameObject promptMarker = fsm.FsmVariables.FindFsmGameObject("Prompt Marker");

            FsmState inert = fsm.GetState("Inert");
            FsmState idle = fsm.GetState("Idle");
            FsmState checkStartState = fsm.GetState("Check Start State");
            FsmState checkBenchState = new(fsm.Fsm)
            {
                Name = "Check Bench State",
                Actions = new FsmStateAction[]
                {
                    new DelegateBoolTest(() => Benchwarp.Benchwarp.LS.visitedBenchScenes.Contains(new(fsm.gameObject.scene.name, fsm.gameObject.name)), "BENCH ACTIVE", null),
                    new DelegateBoolTest(() => info.items.All(i => i.IsObtained()), "WAIT", "GIVE")
                },
                Transitions = new FsmTransition[] { },
            };
            FsmState checkBenchPause = new(fsm.Fsm)
            {
                Name = "Check Bench Pause",
                Actions = new FsmStateAction[]
                {
                    new Wait
                    {
                        finishEvent = FsmEvent.Finished,
                        time = 1f,
                        realTime = false,
                    },
                },
                Transitions = new FsmTransition[] { },
            };

            FsmState idleNoRest = new(fsm.Fsm)
            {
                Name = "Idle-No Rest",
                Actions = new FsmStateAction[]
                {
                    new HidePromptMarker
                    {
                        storedObject = prompt,
                    },
                    new Trigger2dEvent
                    {
                        trigger = PlayMakerUnity2d.Trigger2DType.OnTriggerStay2D,
                        sendEvent = FsmEvent.GetFsmEvent("IN RANGE"),
                        collideTag = string.Empty,
                        storeCollider = new(),
                    }
                },
                Transitions = new FsmTransition[] { }
            };
            FsmState inRangeNoRest = new(fsm.Fsm)
            {
                Name = "In Range-No Rest",
                Actions = new FsmStateAction[] 
                {
                    new ShowPromptMarker
                    {
                        prefab = arrow,
                        labelName = "Accept",
                        spawnPoint = promptMarker,
                        storeObject = prompt,
                    },
                    new Trigger2dEvent
                    {
                        trigger = PlayMakerUnity2d.Trigger2DType.OnTriggerExit2D,
                        sendEvent = FsmEvent.GetFsmEvent("OUT OF RANGE"),
                        collideTag = string.Empty,
                        storeCollider = new(),
                    },
                    new ListenForDown
                    {
                        wasPressed = FsmEvent.GetFsmEvent("UP PRESSED"),
                        isPressedBool = false,
                    },
                    new ListenForUp
                    {
                        wasPressed = FsmEvent.GetFsmEvent("UP PRESSED"),
                        isPressedBool = false,
                    },
                },
                Transitions = new FsmTransition[] { }
            };
            FsmState heroDownNoRest = new(fsm.Fsm)
            {
                Name = "Hero Down-No Rest",
                Actions = new FsmStateAction[]
                {
                    new Lambda(() =>
                    {
                        PlayerData.instance.SetBool(nameof(PlayerData.disablePause), true);
                        HeroController.instance.RelinquishControl();
                        HeroController.instance.StopAnimationControl();
                    }),
                    new HidePromptMarker
                    {
                        storedObject = prompt,
                    },
                    new Tk2dPlayAnimation
                    {
                        gameObject = new()
                        {
                            OwnerOption = OwnerDefaultOption.SpecifyGameObject,
                            GameObject = HeroController.instance.gameObject,
                        },
                        clipName = "Collect Normal 1",
                        animLibName = "",
                    },
                    new Wait
                    {
                        time = 0.75f,
                        realTime = false,
                        finishEvent = FsmEvent.Finished,
                    },
                },
                Transitions = new FsmTransition[] { }
            };
            FsmState giveNoRest = new(fsm.Fsm)
            {
                Name = "Give-No Rest",
                Actions = new FsmStateAction[] 
                {
                    new Tk2dPlayAnimation
                    {
                        gameObject = new()
                        {
                            OwnerOption = OwnerDefaultOption.SpecifyGameObject,
                            GameObject = HeroController.instance.gameObject,
                        },
                        clipName = "Collect Normal 2",
                        animLibName = "",
                    },
                    new AsyncLambda(callback => ItemUtility.GiveSequentially(info.items, info.placement, new GiveInfo
                    {
                        FlingType = info.flingType,
                        Container = Bench,
                        MessageType = MessageType.Any,
                        Transform = fsm.transform,
                    }, callback)),
                },
                Transitions = new FsmTransition[] { }
            };
            FsmState heroUpNoRest = new(fsm.Fsm)
            {
                Name = "Hero Up-No Rest",
                Actions = new FsmStateAction[]
                {
                    new Tk2dPlayAnimationWithEvents
                    {
                        gameObject = new()
                        {
                            OwnerOption = OwnerDefaultOption.SpecifyGameObject,
                            GameObject = HeroController.instance.gameObject,
                        },
                        clipName = "Collect Normal 3",
                        animationCompleteEvent = FsmEvent.Finished,
                    },
                },
                Transitions = new FsmTransition[] { }
            };
            FsmState resetNoRest = new(fsm.Fsm)
            {
                Name = "Reset-No Rest",
                Actions = new FsmStateAction[]
                {
                    new Lambda(() =>
                    {
                        PlayerData.instance.SetBool(nameof(PlayerData.disablePause), false);
                        HeroController.instance.RegainControl();
                        HeroController.instance.StartAnimationControl();
                    }),
                },
                Transitions = new FsmTransition[] { }
            };
            fsm.AddState(checkBenchState);
            fsm.AddState(checkBenchPause);
            fsm.AddState(idleNoRest);
            fsm.AddState(inRangeNoRest);
            fsm.AddState(heroDownNoRest);
            fsm.AddState(giveNoRest);
            fsm.AddState(heroUpNoRest);
            fsm.AddState(resetNoRest);

            checkStartState.Transitions[0].SetToState(checkBenchState);
            inert.Transitions[0].SetToState(checkBenchState);
            checkBenchState.AddTransition("BENCH ACTIVE", idle);
            checkBenchState.AddTransition("GIVE", idleNoRest);
            checkBenchState.AddTransition("WAIT", checkBenchPause);
            checkBenchPause.AddTransition(FsmEvent.Finished, checkBenchState);
            idleNoRest.AddTransition("IN RANGE", inRangeNoRest);
            inRangeNoRest.AddTransition("OUT OF RANGE", idleNoRest);
            inRangeNoRest.AddTransition("UP PRESSED", heroDownNoRest);
            heroDownNoRest.AddTransition(FsmEvent.Finished, giveNoRest);
            giveNoRest.AddTransition(FsmEvent.Finished, heroUpNoRest);
            heroUpNoRest.AddTransition(FsmEvent.Finished, resetNoRest);
            resetNoRest.AddTransition(FsmEvent.Finished, checkBenchState);
        }

        public static void AddGiveEffectAfterRest(PlayMakerFSM fsm, ContainerGiveInfo info)
        {
            FsmState inRange = fsm.GetState("In Range");
            FsmState saveGame = fsm.GetState("Save Game");
            FsmState addToBenchList = fsm.GetState("Add To Bench List");

            FsmState give = new(fsm.Fsm)
            {
                Name = "Give",
                Transitions = new FsmTransition[]
                {
                    new FsmTransition(){ FsmEvent = FsmEvent.Finished, }
                },
                Actions = new FsmStateAction[]
                {
                    new Lambda(() =>
                    {
                        PlayerData.instance.SetBool(nameof(PlayerData.disablePause), true);
                    }),
                    new AsyncLambda(callback => ItemUtility.GiveSequentially(info.items, info.placement, new GiveInfo
                    {
                        FlingType = info.flingType,
                        Container = Bench,
                        MessageType = MessageType.Any,
                        Transform = fsm.transform,
                    }, callback)),
                },
            };
            FsmState giveRecover = new(fsm.Fsm)
            {
                Name = "Give Recover",
                Actions = new FsmStateAction[]
                {
                    new Lambda(() =>
                    {
                        PlayerData.instance.SetBool(nameof(PlayerData.disablePause), false);
                        GameManager.instance.ResetSemiPersistentItems();
                    }),
                },
                Transitions = new FsmTransition[]
                {
                    new FsmTransition()
                    {
                        FsmEvent = FsmEvent.Finished,
                        ToFsmState = addToBenchList,
                        ToState = "Add To Bench List",
                    }
                }
            };

            fsm.AddState(give);
            fsm.AddState(giveRecover);
            saveGame.Transitions[0].SetToState(give);
            give.Transitions[0].SetToState(giveRecover);

            inRange.AddFirstAction(new Lambda(() =>
            {
                if (info.items.Any(i => !i.IsObtained()))
                {
                    inRange.GetFirstActionOfType<ShowPromptMarker>().labelName = "Accept";
                }
                else
                {
                    inRange.GetFirstActionOfType<ShowPromptMarker>().labelName = "Rest";
                }
            }));
        }
    }
}
