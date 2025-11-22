import { describe, it, expect, beforeEach, vi } from 'vitest';
import { createPKHeXApiWrapper } from '../src/api-wrapper';

describe('API Wrapper', () => {
  let mockRawApi: any;
  let api: any;

  beforeEach(() => {
    mockRawApi = {
      LoadSave: vi.fn(),
      GetSaveInfo: vi.fn(),
      ExportSave: vi.fn(),
      DisposeSave: vi.fn(),
      GetAllPokemon: vi.fn(),
      GetParty: vi.fn(),
      GetPokemon: vi.fn(),
      GetPartySlot: vi.fn(),
      ModifyPokemon: vi.fn(),
      SetPokemon: vi.fn(),
      DeletePokemon: vi.fn(),
      MovePokemon: vi.fn(),
      GeneratePID: vi.fn(),
      SetPID: vi.fn(),
      SetShiny: vi.fn(),
      GetPIDInfo: vi.fn(),
      CheckLegality: vi.fn(),
      LegalizePokemon: vi.fn(),
      ExportShowdown: vi.fn(),
      ImportShowdown: vi.fn(),
      GetRibbons: vi.fn(),
      GetRibbonCount: vi.fn(),
      SetRibbon: vi.fn(),
      GetContestStats: vi.fn(),
      SetContestStat: vi.fn(),
      GetTrainerInfo: vi.fn(),
      SetTrainerInfo: vi.fn(),
      GetTrainerCard: vi.fn(),
      GetTrainerAppearance: vi.fn(),
      SetTrainerAppearance: vi.fn(),
      GetRivalName: vi.fn(),
      SetRivalName: vi.fn(),
      GetBadges: vi.fn(),
      SetBadge: vi.fn(),
      GetBoxNames: vi.fn(),
      GetBoxWallpapers: vi.fn(),
      SetBoxWallpaper: vi.fn(),
      GetBattleBox: vi.fn(),
      SetBattleBoxSlot: vi.fn(),
      GetDaycare: vi.fn(),
      GetPouchItems: vi.fn(),
      AddItemToPouch: vi.fn(),
      RemoveItemFromPouch: vi.fn(),
      GetPokedex: vi.fn(),
      SetPokedexSeen: vi.fn(),
      SetPokedexCaught: vi.fn(),
      GetBattlePoints: vi.fn(),
      SetBattlePoints: vi.fn(),
      GetCoins: vi.fn(),
      SetCoins: vi.fn(),
      GetRecords: vi.fn(),
      SetRecord: vi.fn(),
      GetEventFlag: vi.fn(),
      SetEventFlag: vi.fn(),
      GetEventConst: vi.fn(),
      SetEventConst: vi.fn(),
      GetBattleFacilityStats: vi.fn(),
      GetHallOfFame: vi.fn(),
      SetHallOfFameEntry: vi.fn(),
      GetSecondsPlayed: vi.fn(),
      GetSecondsToStart: vi.fn(),
      GetSecondsToFame: vi.fn(),
      SetGameTime: vi.fn(),
      SetSecondsToStart: vi.fn(),
      SetSecondsToFame: vi.fn(),
      GetMailbox: vi.fn(),
      GetMailMessage: vi.fn(),
      SetMailMessage: vi.fn(),
      DeleteMail: vi.fn(),
      GetMysteryGifts: vi.fn(),
      GetMysteryGiftCard: vi.fn(),
      SetMysteryGiftCard: vi.fn(),
      GetMysteryGiftFlags: vi.fn(),
      DeleteMysteryGift: vi.fn(),
      GetSecretBase: vi.fn(),
      GetEntralinkData: vi.fn(),
      GetPokePelago: vi.fn(),
      GetFestivalPlaza: vi.fn(),
      GetPokeJobs: vi.fn(),
      GetSpeciesName: vi.fn(),
      GetAllSpecies: vi.fn(),
      GetMoveName: vi.fn(),
      GetAllMoves: vi.fn(),
      GetAbilityName: vi.fn(),
      GetAllAbilities: vi.fn(),
      GetItemName: vi.fn(),
      GetAllItems: vi.fn(),
      GetNatureName: vi.fn(),
      GetAllNatures: vi.fn(),
      GetTypeName: vi.fn(),
      GetAllTypes: vi.fn(),
      CollectColorfulScrews: vi.fn(),
      GetColorfulScrewLocations: vi.fn(),
      GetInfiniteRoyalePoints: vi.fn(),
      SetInfiniteRoyalePoints: vi.fn(),
      SetTextSpeed: vi.fn(),
      GetTextSpeed: vi.fn(),
      UnlockFashionCategory: vi.fn(),
      UnlockAllFashion: vi.fn(),
      UnlockAllHairMakeup: vi.fn(),
      GetPKMData: vi.fn(),
      ModifyPKMData: vi.fn(),
      CheckPKMLegality: vi.fn(),
      LegalizePKMData: vi.fn(),
      ExportPKMShowdown: vi.fn(),
      CalculatePKMStats: vi.fn(),
      GetPKMRibbons: vi.fn(),
      SetPKMRibbon: vi.fn(),
      SetPKMShiny: vi.fn(),
      GetPKMPIDInfo: vi.fn(),
      SetPKMPID: vi.fn(),
    };
    api = createPKHeXApiWrapper(mockRawApi);
  });

  describe('Static Utility Functions', () => {
    it('getSpeciesName should parse JSON response', () => {
      const mockResponse = JSON.stringify({ name: 'Pikachu' });
      mockRawApi.GetSpeciesName.mockReturnValue(mockResponse);

      const result = api.getSpeciesName(25);

      expect(mockRawApi.GetSpeciesName).toHaveBeenCalledWith(25);
      expect(result).toEqual({ name: 'Pikachu' });
    });

    it('getAllSpecies should return species array', () => {
      const mockResponse = JSON.stringify({ species: [{ id: 1, name: 'Bulbasaur' }] });
      mockRawApi.GetAllSpecies.mockReturnValue(mockResponse);

      const result = api.getAllSpecies();

      expect(mockRawApi.GetAllSpecies).toHaveBeenCalled();
      expect(result).toHaveProperty('species');
    });

    it('getMoveName should parse JSON response', () => {
      const mockResponse = JSON.stringify({ name: 'Thunderbolt' });
      mockRawApi.GetMoveName.mockReturnValue(mockResponse);

      const result = api.getMoveName(85);

      expect(mockRawApi.GetMoveName).toHaveBeenCalledWith(85);
      expect(result).toEqual({ name: 'Thunderbolt' });
    });

    it('getAllMoves should return moves array', () => {
      const mockResponse = JSON.stringify({ moves: [{ id: 1, name: 'Pound' }] });
      mockRawApi.GetAllMoves.mockReturnValue(mockResponse);

      const result = api.getAllMoves();

      expect(mockRawApi.GetAllMoves).toHaveBeenCalled();
      expect(result).toHaveProperty('moves');
    });
  });

  describe('Save Operations', () => {
    it('save.load should parse JSON response', () => {
      const mockResponse = JSON.stringify({ success: true, handle: 123 });
      mockRawApi.LoadSave.mockReturnValue(mockResponse);

      const result = api.save.load('base64data');

      expect(mockRawApi.LoadSave).toHaveBeenCalledWith('base64data');
      expect(result).toEqual({ success: true, handle: 123 });
    });

    it('save.getInfo should parse JSON response', () => {
      const mockResponse = JSON.stringify({
        success: true,
        generation: 'Gen 3',
        gameVersion: 'Emerald',
        ot: 'ASH',
        tid: 12345,
        sid: 54321,
        boxCount: 14
      });
      mockRawApi.GetSaveInfo.mockReturnValue(mockResponse);

      const result = api.save.getInfo(1);

      expect(mockRawApi.GetSaveInfo).toHaveBeenCalledWith(1);
      expect(result).toHaveProperty('success', true);
    });

    it('save.export should parse JSON response', () => {
      const mockResponse = JSON.stringify({ base64Data: 'exporteddata' });
      mockRawApi.ExportSave.mockReturnValue(mockResponse);

      const result = api.save.export(1);

      expect(mockRawApi.ExportSave).toHaveBeenCalledWith(1);
      expect(result).toEqual({ base64Data: 'exporteddata' });
    });

    it('save.dispose should call raw API', () => {
      const mockResponse = JSON.stringify({ success: true, message: 'Disposed' });
      mockRawApi.DisposeSave.mockReturnValue(mockResponse);

      const result = api.save.dispose(1);

      expect(mockRawApi.DisposeSave).toHaveBeenCalledWith(1);
      expect(result).toHaveProperty('success', true);
    });
  });

  describe('Pokemon Operations', () => {
    it('save.pokemon.get should return PokemonDetail', () => {
      const mockResponse = JSON.stringify({
        species: 25,
        speciesName: 'Pikachu',
        level: 50,
        moves: [85, 86, 87, 88]
      });
      mockRawApi.GetPokemon.mockReturnValue(mockResponse);

      const result = api.save.pokemon.get(1, 0, 0);

      expect(mockRawApi.GetPokemon).toHaveBeenCalledWith(1, 0, 0);
      expect(result).toHaveProperty('species', 25);
    });

    it('save.pokemon.getAll should return array', () => {
      const mockResponse = JSON.stringify([
        { box: 0, slot: 0, species: 25, speciesName: 'Pikachu', level: 50, isEgg: false, isShiny: false }
      ]);
      mockRawApi.GetAllPokemon.mockReturnValue(mockResponse);

      const result = api.save.pokemon.getAll(1);

      expect(mockRawApi.GetAllPokemon).toHaveBeenCalledWith(1);
      expect(Array.isArray(result)).toBe(true);
    });

    it('save.pokemon.getParty should return party array', () => {
      const mockResponse = JSON.stringify([
        { box: -1, slot: 0, species: 25, speciesName: 'Pikachu', level: 50, isEgg: false, isShiny: false }
      ]);
      mockRawApi.GetParty.mockReturnValue(mockResponse);

      const result = api.save.pokemon.getParty(1);

      expect(mockRawApi.GetParty).toHaveBeenCalledWith(1);
      expect(Array.isArray(result)).toBe(true);
    });

    it('save.pokemon.getPartySlot should return PokemonDetail', () => {
      const mockResponse = JSON.stringify({
        species: 25,
        speciesName: 'Pikachu',
        level: 50
      });
      mockRawApi.GetPartySlot.mockReturnValue(mockResponse);

      const result = api.save.pokemon.getPartySlot(1, 0);


      expect(mockRawApi.GetPartySlot).toHaveBeenCalledWith(1, 0);
      expect(result).toHaveProperty('species', 25);
    });

    it('save.pokemon.modify should call raw API', () => {
      const mockResponse = JSON.stringify({ success: true, message: 'Modified' });
      mockRawApi.ModifyPokemon.mockReturnValue(mockResponse);

      const result = api.save.pokemon.modify(1, 0, 0, { level: 100 });

      expect(mockRawApi.ModifyPokemon).toHaveBeenCalledWith(1, 0, 0, '{"level":100}');
      expect(result).toHaveProperty('success', true);
    });

    it('save.pokemon.set should call raw API', () => {
      const mockResponse = JSON.stringify({ success: true, message: 'Set' });
      mockRawApi.SetPokemon.mockReturnValue(mockResponse);

      const result = api.save.pokemon.set(1, 0, 0, 'base64data');

      expect(mockRawApi.SetPokemon).toHaveBeenCalledWith(1, 0, 0, 'base64data');
      expect(result).toHaveProperty('success', true);
    });

    it('save.pokemon.delete should call raw API', () => {
      const mockResponse = JSON.stringify({ success: true, message: 'Deleted' });
      mockRawApi.DeletePokemon.mockReturnValue(mockResponse);

      const result = api.save.pokemon.delete(1, 0, 0);

      expect(mockRawApi.DeletePokemon).toHaveBeenCalledWith(1, 0, 0);
      expect(result).toHaveProperty('success', true);
    });

    it('save.pokemon.move should call raw API', () => {
      const mockResponse = JSON.stringify({ success: true, message: 'Moved' });
      mockRawApi.MovePokemon.mockReturnValue(mockResponse);

      const result = api.save.pokemon.move(1, 0, 0, 1, 5);

      expect(mockRawApi.MovePokemon).toHaveBeenCalledWith(1, 0, 0, 1, 5);
      expect(result).toHaveProperty('success', true);
    });

    it('save.pokemon.generatePID should call raw API', () => {
      const mockResponse = JSON.stringify({ success: true, message: 'PID generated' });
      mockRawApi.GeneratePID.mockReturnValue(mockResponse);

      const result = api.save.pokemon.generatePID(1, 0, 0, 5, true);


      expect(mockRawApi.GeneratePID).toHaveBeenCalledWith(1, 0, 0, 5, true);
      expect(result).toHaveProperty('success', true);
    });

    it('save.pokemon.setPID should call raw API', () => {
      const mockResponse = JSON.stringify({ success: true, message: 'PID set' });
      mockRawApi.SetPID.mockReturnValue(mockResponse);

      const result = api.save.pokemon.setPID(1, 0, 0, 12345);

      expect(mockRawApi.SetPID).toHaveBeenCalledWith(1, 0, 0, 12345);
      expect(result).toHaveProperty('success', true);
    });

    it('save.pokemon.setShiny should call raw API', () => {
      const mockResponse = JSON.stringify({ success: true, message: 'Shiny set' });
      mockRawApi.SetShiny.mockReturnValue(mockResponse);

      const result = api.save.pokemon.setShiny(1, 0, 0, 1);

      expect(mockRawApi.SetShiny).toHaveBeenCalledWith(1, 0, 0, 1);
      expect(result).toHaveProperty('success', true);
    });

    it('save.pokemon.getPIDInfo should return PID info', () => {
      const mockResponse = JSON.stringify({ pid: 12345, nature: 5, isShiny: true });
      mockRawApi.GetPIDInfo.mockReturnValue(mockResponse);

      const result = api.save.pokemon.getPIDInfo(1, 0, 0);

      expect(mockRawApi.GetPIDInfo).toHaveBeenCalledWith(1, 0, 0);
      expect(result).toHaveProperty('pid', 12345);
    });

    it('save.pokemon.checkLegality should return legality result', () => {
      const mockResponse = JSON.stringify({ valid: true, report: 'Legal' });
      mockRawApi.CheckLegality.mockReturnValue(mockResponse);

      const result = api.save.pokemon.checkLegality(1, 0, 0);

      expect(mockRawApi.CheckLegality).toHaveBeenCalledWith(1, 0, 0);
      expect(result).toHaveProperty('valid', true);
    });

    it('save.pokemon.legalize should call raw API', () => {
      const mockResponse = JSON.stringify({ success: true, message: 'Legalized' });
      mockRawApi.LegalizePokemon.mockReturnValue(mockResponse);

      const result = api.save.pokemon.legalize(1, 0, 0);


      expect(mockRawApi.LegalizePokemon).toHaveBeenCalledWith(1, 0, 0);
      expect(result).toHaveProperty('success', true);
    });

    it('save.pokemon.exportShowdown should return showdown text', () => {
      const mockResponse = JSON.stringify({ showdownText: 'Pikachu @ Light Ball' });
      mockRawApi.ExportShowdown.mockReturnValue(mockResponse);

      const result = api.save.pokemon.exportShowdown(1, 0, 0);

      expect(mockRawApi.ExportShowdown).toHaveBeenCalledWith(1, 0, 0);
      expect(result).toHaveProperty('showdownText');
    });

    it('save.pokemon.importShowdown should call raw API', () => {
      const mockResponse = JSON.stringify({ success: true, message: 'Imported' });
      mockRawApi.ImportShowdown.mockReturnValue(mockResponse);

      const result = api.save.pokemon.importShowdown(1, 0, 0, 'Pikachu @ Light Ball');

      expect(mockRawApi.ImportShowdown).toHaveBeenCalledWith(1, 0, 0, 'Pikachu @ Light Ball');
      expect(result).toHaveProperty('success', true);
    });

    it('save.pokemon.getRibbons should return ribbons array', () => {
      const mockResponse = JSON.stringify([{ ribbonName: 'Champion', hasRibbon: true }]);
      mockRawApi.GetRibbons.mockReturnValue(mockResponse);

      const result = api.save.pokemon.getRibbons(1, 0, 0);

      expect(mockRawApi.GetRibbons).toHaveBeenCalledWith(1, 0, 0);
      expect(Array.isArray(result)).toBe(true);
    });

    it('save.pokemon.getRibbonCount should return count', () => {
      const mockResponse = JSON.stringify({ ribbonCount: 5 });
      mockRawApi.GetRibbonCount.mockReturnValue(mockResponse);

      const result = api.save.pokemon.getRibbonCount(1, 0, 0);

      expect(mockRawApi.GetRibbonCount).toHaveBeenCalledWith(1, 0, 0);
      expect(result).toHaveProperty('ribbonCount', 5);
    });

    it('save.pokemon.setRibbon should call raw API', () => {
      const mockResponse = JSON.stringify({ success: true, message: 'Ribbon set' });
      mockRawApi.SetRibbon.mockReturnValue(mockResponse);

      const result = api.save.pokemon.setRibbon(1, 0, 0, 'Champion', true);


      expect(mockRawApi.SetRibbon).toHaveBeenCalledWith(1, 0, 0, 'Champion', true);
      expect(result).toHaveProperty('success', true);
    });

    it('save.pokemon.getContestStats should return contest stats', () => {
      const mockResponse = JSON.stringify({ cool: 50, beauty: 60, cute: 70 });
      mockRawApi.GetContestStats.mockReturnValue(mockResponse);

      const result = api.save.pokemon.getContestStats(1, 0, 0);

      expect(mockRawApi.GetContestStats).toHaveBeenCalledWith(1, 0, 0);
      expect(result).toHaveProperty('cool', 50);
    });

    it('save.pokemon.setContestStat should call raw API', () => {
      const mockResponse = JSON.stringify({ success: true, message: 'Contest stat set' });
      mockRawApi.SetContestStat.mockReturnValue(mockResponse);

      const result = api.save.pokemon.setContestStat(1, 0, 0, 'cool', 100);

      expect(mockRawApi.SetContestStat).toHaveBeenCalledWith(1, 0, 0, 'cool', 100);
      expect(result).toHaveProperty('success', true);
    });
  });

  describe('Trainer Operations', () => {
    it('save.trainer.getInfo should return trainer info', () => {
      const mockResponse = JSON.stringify({ ot: 'ASH', tid: 12345, sid: 54321 });
      mockRawApi.GetTrainerInfo.mockReturnValue(mockResponse);

      const result = api.save.trainer.getInfo(1);

      expect(mockRawApi.GetTrainerInfo).toHaveBeenCalledWith(1);
      expect(result).toHaveProperty('ot', 'ASH');
    });

    it('save.trainer.setInfo should call raw API', () => {
      const mockResponse = JSON.stringify({ success: true, message: 'Trainer info set' });
      mockRawApi.SetTrainerInfo.mockReturnValue(mockResponse);

      const result = api.save.trainer.setInfo(1, { ot: "RED", tid: 12345, sid: 0, gender: 0, language: 1, money: 50000, playedHours: 10, playedMinutes: 30, playedSeconds: 45 });

      expect(mockRawApi.SetTrainerInfo).toHaveBeenCalledWith(1, '{"ot":"RED","tid":12345,"sid":0,"gender":0,"language":1,"money":50000,"playedHours":10,"playedMinutes":30,"playedSeconds":45}');
      expect(result).toHaveProperty('success', true);
    });

    it('save.trainer.getCard should return trainer card', () => {
      const mockResponse = JSON.stringify({ ot: 'ASH', badges: 8 });
      mockRawApi.GetTrainerCard.mockReturnValue(mockResponse);

      const result = api.save.trainer.getCard(1);


      expect(mockRawApi.GetTrainerCard).toHaveBeenCalledWith(1);
      expect(result).toHaveProperty('ot', 'ASH');
    });

    it('save.trainer.getAppearance should return appearance', () => {
      const mockResponse = JSON.stringify({ gender: 0, skin: 1 });
      mockRawApi.GetTrainerAppearance.mockReturnValue(mockResponse);

      const result = api.save.trainer.getAppearance(1);

      expect(mockRawApi.GetTrainerAppearance).toHaveBeenCalledWith(1);
      expect(result).toHaveProperty('gender', 0);
    });

    it('save.trainer.setAppearance should call raw API', () => {
      const mockResponse = JSON.stringify({ success: true, message: 'Appearance set' });
      mockRawApi.SetTrainerAppearance.mockReturnValue(mockResponse);

      const result = api.save.trainer.setAppearance(1, { skin: 0, hair: 1, top: 2, bottom: 3, shoes: 4, accessory: 5, bag: 6, hat: 7 });

      expect(mockRawApi.SetTrainerAppearance).toHaveBeenCalledWith(1, '{"skin":0,"hair":1,"top":2,"bottom":3,"shoes":4,"accessory":5,"bag":6,"hat":7}');
      expect(result).toHaveProperty('success', true);
    });

    it('save.trainer.getRivalName should return rival name', () => {
      const mockResponse = JSON.stringify({ rivalName: 'BLUE' });
      mockRawApi.GetRivalName.mockReturnValue(mockResponse);

      const result = api.save.trainer.getRivalName(1);

      expect(mockRawApi.GetRivalName).toHaveBeenCalledWith(1);
      expect(result).toHaveProperty('rivalName', 'BLUE');
    });

    it('save.trainer.setRivalName should call raw API', () => {
      const mockResponse = JSON.stringify({ success: true, message: 'Rival name set' });
      mockRawApi.SetRivalName.mockReturnValue(mockResponse);

      const result = api.save.trainer.setRivalName(1, 'GARY');

      expect(mockRawApi.SetRivalName).toHaveBeenCalledWith(1, 'GARY');
      expect(result).toHaveProperty('success', true);
    });

    it('save.trainer.getBadges should return badge data', () => {
      const mockResponse = JSON.stringify({ badgeCount: 8, badges: [true, true] });
      mockRawApi.GetBadges.mockReturnValue(mockResponse);

      const result = api.save.trainer.getBadges(1);

      expect(mockRawApi.GetBadges).toHaveBeenCalledWith(1);
      expect(result).toHaveProperty('badgeCount', 8);
    });


    it('save.trainer.setBadge should call raw API', () => {
      const mockResponse = JSON.stringify({ success: true, message: 'Badge set' });
      mockRawApi.SetBadge.mockReturnValue(mockResponse);

      const result = api.save.trainer.setBadge(1, 0, true);

      expect(mockRawApi.SetBadge).toHaveBeenCalledWith(1, 0, true);
      expect(result).toHaveProperty('success', true);
    });
  });

  describe('Box Operations', () => {
    it('save.boxes.getNames should return box names', () => {
      const mockResponse = JSON.stringify(['Box 1', 'Box 2']);
      mockRawApi.GetBoxNames.mockReturnValue(mockResponse);

      const result = api.save.boxes.getNames(1);

      expect(mockRawApi.GetBoxNames).toHaveBeenCalledWith(1);
      expect(Array.isArray(result)).toBe(true);
    });

    it('save.boxes.getWallpapers should return wallpaper info', () => {
      const mockResponse = JSON.stringify([{ boxIndex: 0, wallpaperId: 1 }]);
      mockRawApi.GetBoxWallpapers.mockReturnValue(mockResponse);

      const result = api.save.boxes.getWallpapers(1);

      expect(mockRawApi.GetBoxWallpapers).toHaveBeenCalledWith(1);
      expect(Array.isArray(result)).toBe(true);
    });

    it('save.boxes.setWallpaper should call raw API', () => {
      const mockResponse = JSON.stringify({ success: true, message: 'Wallpaper set' });
      mockRawApi.SetBoxWallpaper.mockReturnValue(mockResponse);

      const result = api.save.boxes.setWallpaper(1, 0, 5);

      expect(mockRawApi.SetBoxWallpaper).toHaveBeenCalledWith(1, 0, 5);
      expect(result).toHaveProperty('success', true);
    });

    it('save.boxes.getBattleBox should return battle box', () => {
      const mockResponse = JSON.stringify([{ species: 25, level: 50 }]);
      mockRawApi.GetBattleBox.mockReturnValue(mockResponse);

      const result = api.save.boxes.getBattleBox(1);

      expect(mockRawApi.GetBattleBox).toHaveBeenCalledWith(1);
      expect(Array.isArray(result)).toBe(true);
    });

    it('save.boxes.setBattleBoxSlot should call raw API', () => {
      const mockResponse = JSON.stringify({ success: true, message: 'Battle box slot set' });
      mockRawApi.SetBattleBoxSlot.mockReturnValue(mockResponse);


      const result = api.save.boxes.setBattleBoxSlot(1, 0, 'base64data');

      expect(mockRawApi.SetBattleBoxSlot).toHaveBeenCalledWith(1, 0, 'base64data');
      expect(result).toHaveProperty('success', true);
    });

    it('save.boxes.getDaycare should return daycare data', () => {
      const mockResponse = JSON.stringify({ slot1: null, slot2: null });
      mockRawApi.GetDaycare.mockReturnValue(mockResponse);

      const result = api.save.boxes.getDaycare(1);

      expect(mockRawApi.GetDaycare).toHaveBeenCalledWith(1);
      expect(result).toBeDefined();
    });
  });

  describe('Item Operations', () => {
    it('save.items.getPouches should return pouch data', () => {
      const mockResponse = JSON.stringify([{ pouchIndex: 0, items: [] }]);
      mockRawApi.GetPouchItems.mockReturnValue(mockResponse);

      const result = api.save.items.getPouches(1);

      expect(mockRawApi.GetPouchItems).toHaveBeenCalledWith(1);
      expect(Array.isArray(result)).toBe(true);
    });

    it('save.items.add should call raw API', () => {
      const mockResponse = JSON.stringify({ success: true, message: 'Item added' });
      mockRawApi.AddItemToPouch.mockReturnValue(mockResponse);

      const result = api.save.items.add(1, 1, 10, 0);

      expect(mockRawApi.AddItemToPouch).toHaveBeenCalledWith(1, 1, 10, 0);
      expect(result).toHaveProperty('success', true);
    });

    it('save.items.remove should call raw API', () => {
      const mockResponse = JSON.stringify({ success: true, message: 'Item removed' });
      mockRawApi.RemoveItemFromPouch.mockReturnValue(mockResponse);

      const result = api.save.items.remove(1, 1, 5);

      expect(mockRawApi.RemoveItemFromPouch).toHaveBeenCalledWith(1, 1, 5);
      expect(result).toHaveProperty('success', true);
    });
  });

  describe('Pokedex Operations', () => {
    it('save.pokedex.get should return pokedex entries', () => {
      const mockResponse = JSON.stringify([{ species: 1, seen: true, caught: true }]);
      mockRawApi.GetPokedex.mockReturnValue(mockResponse);

      const result = api.save.pokedex.get(1);


      expect(mockRawApi.GetPokedex).toHaveBeenCalledWith(1);
      expect(Array.isArray(result)).toBe(true);
    });

    it('save.pokedex.setSeen should call raw API', () => {
      const mockResponse = JSON.stringify({ success: true, message: 'Seen set' });
      mockRawApi.SetPokedexSeen.mockReturnValue(mockResponse);

      const result = api.save.pokedex.setSeen(1, 25, 0);

      expect(mockRawApi.SetPokedexSeen).toHaveBeenCalledWith(1, 25, 0);
      expect(result).toHaveProperty('success', true);
    });

    it('save.pokedex.setCaught should call raw API', () => {
      const mockResponse = JSON.stringify({ success: true, message: 'Caught set' });
      mockRawApi.SetPokedexCaught.mockReturnValue(mockResponse);

      const result = api.save.pokedex.setCaught(1, 25, 0);

      expect(mockRawApi.SetPokedexCaught).toHaveBeenCalledWith(1, 25, 0);
      expect(result).toHaveProperty('success', true);
    });
  });

  describe('Progress Operations', () => {
    it('save.progress.getBattlePoints should return battle points', () => {
      const mockResponse = JSON.stringify({ battlePoints: 100 });
      mockRawApi.GetBattlePoints.mockReturnValue(mockResponse);

      const result = api.save.progress.getBattlePoints(1);

      expect(mockRawApi.GetBattlePoints).toHaveBeenCalledWith(1);
      expect(result).toHaveProperty('battlePoints', 100);
    });

    it('save.progress.setBattlePoints should call raw API', () => {
      const mockResponse = JSON.stringify({ success: true, message: 'Battle points set' });
      mockRawApi.SetBattlePoints.mockReturnValue(mockResponse);

      const result = api.save.progress.setBattlePoints(1, 200);

      expect(mockRawApi.SetBattlePoints).toHaveBeenCalledWith(1, 200);
      expect(result).toHaveProperty('success', true);
    });

    it('save.progress.getCoins should return coins', () => {
      const mockResponse = JSON.stringify({ coins: 500 });
      mockRawApi.GetCoins.mockReturnValue(mockResponse);

      const result = api.save.progress.getCoins(1);

      expect(mockRawApi.GetCoins).toHaveBeenCalledWith(1);
      expect(result).toHaveProperty('coins', 500);
    });


    it('save.progress.setCoins should call raw API', () => {
      const mockResponse = JSON.stringify({ success: true, message: 'Coins set' });
      mockRawApi.SetCoins.mockReturnValue(mockResponse);

      const result = api.save.progress.setCoins(1, 1000);

      expect(mockRawApi.SetCoins).toHaveBeenCalledWith(1, 1000);
      expect(result).toHaveProperty('success', true);
    });

    it('save.progress.getEventFlag should return flag value', () => {
      const mockResponse = JSON.stringify({ flagIndex: 100, value: true });
      mockRawApi.GetEventFlag.mockReturnValue(mockResponse);

      const result = api.save.progress.getEventFlag(1, 100);

      expect(mockRawApi.GetEventFlag).toHaveBeenCalledWith(1, 100);
      expect(result).toHaveProperty('value', true);
    });

    it('save.progress.setEventFlag should call raw API', () => {
      const mockResponse = JSON.stringify({ success: true, message: 'Event flag set' });
      mockRawApi.SetEventFlag.mockReturnValue(mockResponse);

      const result = api.save.progress.setEventFlag(1, 100, true);

      expect(mockRawApi.SetEventFlag).toHaveBeenCalledWith(1, 100, true);
      expect(result).toHaveProperty('success', true);
    });
  });

  describe('Time Operations', () => {
    it('save.time.getSecondsPlayed should return seconds', () => {
      const mockResponse = JSON.stringify({ secondsPlayed: 3600 });
      mockRawApi.GetSecondsPlayed.mockReturnValue(mockResponse);

      const result = api.save.time.getSecondsPlayed(1);

      expect(mockRawApi.GetSecondsPlayed).toHaveBeenCalledWith(1);
      expect(result).toHaveProperty('secondsPlayed', 3600);
    });

    it('save.time.setGameTime should call raw API', () => {
      const mockResponse = JSON.stringify({ success: true, message: 'Game time set' });
      mockRawApi.SetGameTime.mockReturnValue(mockResponse);

      const result = api.save.time.setGameTime(1, 10, 30, 45);

      expect(mockRawApi.SetGameTime).toHaveBeenCalledWith(1, 10, 30, 45);
      expect(result).toHaveProperty('success', true);
    });
  });

  describe('Standalone PKM Operations', () => {
    it('pkm.setShiny should call raw API', () => {
      const mockResponse = JSON.stringify({ base64Data: 'modifieddata' });
      mockRawApi.SetPKMShiny.mockReturnValue(mockResponse);


      const result = api.pkm.setShiny('base64data', 3, 1);

      expect(mockRawApi.SetPKMShiny).toHaveBeenCalledWith('base64data', 3, 1);
      expect(result).toHaveProperty('base64Data');
    });

    it('pkm.getPIDInfo should return PID info', () => {
      const mockResponse = JSON.stringify({ pid: 12345, nature: 5, isShiny: true });
      mockRawApi.GetPKMPIDInfo.mockReturnValue(mockResponse);

      const result = api.pkm.getPIDInfo('base64data', 3);

      expect(mockRawApi.GetPKMPIDInfo).toHaveBeenCalledWith('base64data', 3);
      expect(result).toHaveProperty('pid', 12345);
    });

    it('pkm.setPID should call raw API', () => {
      const mockResponse = JSON.stringify({ base64Data: 'modifieddata' });
      mockRawApi.SetPKMPID.mockReturnValue(mockResponse);

      const result = api.pkm.setPID('base64data', 3, 12345);

      expect(mockRawApi.SetPKMPID).toHaveBeenCalledWith('base64data', 3, 12345);
      expect(result).toHaveProperty('base64Data');
    });

    it('pkm.getData should return Pokemon details', () => {
      const mockResponse = JSON.stringify({
        species: 25,
        speciesName: 'Pikachu',
        level: 50,
        moves: [85, 86, 87, 88]
      });
      mockRawApi.GetPKMData.mockReturnValue(mockResponse);

      const result = api.pkm.getData('base64data', 3);

      expect(mockRawApi.GetPKMData).toHaveBeenCalledWith('base64data', 3);
      expect(result).toHaveProperty('species', 25);
      expect(result).toHaveProperty('speciesName', 'Pikachu');
    });

    it('pkm.modify should modify Pokemon data', () => {
      const mockResponse = JSON.stringify({ base64Data: 'modifieddata' });
      mockRawApi.ModifyPKMData.mockReturnValue(mockResponse);

      const result = api.pkm.modify('base64data', 3, { level: 100 });

      expect(mockRawApi.ModifyPKMData).toHaveBeenCalledWith('base64data', 3, '{"level":100}');
      expect(result).toHaveProperty('base64Data');
    });

    it('pkm.checkLegality should return legality result', () => {
      const mockResponse = JSON.stringify({ valid: true, errors: [] });
      mockRawApi.CheckPKMLegality.mockReturnValue(mockResponse);

      const result = api.pkm.checkLegality('base64data', 3);

      expect(mockRawApi.CheckPKMLegality).toHaveBeenCalledWith('base64data', 3);
      expect(result).toHaveProperty('valid', true);
    });

    it('pkm.legalize should legalize Pokemon', () => {
      const mockResponse = JSON.stringify({ base64Data: 'legalizeddata' });
      mockRawApi.LegalizePKMData.mockReturnValue(mockResponse);

      const result = api.pkm.legalize('base64data', 3);

      expect(mockRawApi.LegalizePKMData).toHaveBeenCalledWith('base64data', 3);
      expect(result).toHaveProperty('base64Data');
    });

    it('pkm.exportShowdown should return showdown text', () => {
      const mockResponse = JSON.stringify({ showdownText: 'Pikachu @ Light Ball' });
      mockRawApi.ExportPKMShowdown.mockReturnValue(mockResponse);

      const result = api.pkm.exportShowdown('base64data', 3);

      expect(mockRawApi.ExportPKMShowdown).toHaveBeenCalledWith('base64data', 3);
      expect(result).toHaveProperty('showdownText');
    });

    it('pkm.calculateStats should return calculated stats', () => {
      const mockResponse = JSON.stringify({ hp: 100, attack: 80, defense: 70, spAttack: 90, spDefense: 75, speed: 110 });
      mockRawApi.CalculatePKMStats.mockReturnValue(mockResponse);

      const result = api.pkm.calculateStats('base64data', 3);

      expect(mockRawApi.CalculatePKMStats).toHaveBeenCalledWith('base64data', 3);
      expect(result).toHaveProperty('hp', 100);
      expect(result).toHaveProperty('speed', 110);
    });

    it('pkm.getRibbons should return ribbons array', () => {
      const mockResponse = JSON.stringify([{ name: 'Champion', hasRibbon: true }]);
      mockRawApi.GetPKMRibbons.mockReturnValue(mockResponse);

      const result = api.pkm.getRibbons('base64data', 3);

      expect(mockRawApi.GetPKMRibbons).toHaveBeenCalledWith('base64data', 3);
      expect(Array.isArray(result)).toBe(true);
    });

    it('pkm.setRibbon should set ribbon', () => {
      const mockResponse = JSON.stringify({ base64Data: 'modifieddata' });
      mockRawApi.SetPKMRibbon.mockReturnValue(mockResponse);

      const result = api.pkm.setRibbon('base64data', 3, 'Champion', true);

      expect(mockRawApi.SetPKMRibbon).toHaveBeenCalledWith('base64data', 3, 'Champion', true);
      expect(result).toHaveProperty('base64Data');
    });
  });

  describe('Save Progress Methods', () => {
    it('save.progress.collectColorfulScrews should call CollectColorfulScrews', () => {
      const mockResponse = JSON.stringify({ screwsCollected: 100 });
      mockRawApi.CollectColorfulScrews.mockReturnValue(mockResponse);

      const result = api.save.progress.collectColorfulScrews(1);

      expect(mockRawApi.CollectColorfulScrews).toHaveBeenCalledWith(1);
      expect(result).toHaveProperty('screwsCollected', 100);
    });

    it('save.progress.getColorfulScrewLocations should handle boolean parameter', () => {
      const mockResponse = JSON.stringify({ locations: [] });
      mockRawApi.GetColorfulScrewLocations.mockReturnValue(mockResponse);

      const result = api.save.progress.getColorfulScrewLocations(1, true);

      expect(mockRawApi.GetColorfulScrewLocations).toHaveBeenCalledWith(1, true);
      expect(result).toHaveProperty('locations');
    });

    it('save.progress.getInfiniteRoyalePoints should call GetInfiniteRoyalePoints', () => {
      const mockResponse = JSON.stringify({ royalePoints: 1000, infinitePoints: 500 });
      mockRawApi.GetInfiniteRoyalePoints.mockReturnValue(mockResponse);

      const result = api.save.progress.getInfiniteRoyalePoints(1);

      expect(mockRawApi.GetInfiniteRoyalePoints).toHaveBeenCalledWith(1);
      expect(result).toHaveProperty('royalePoints', 1000);
      expect(result).toHaveProperty('infinitePoints', 500);
    });

    it('save.progress.setInfiniteRoyalePoints should handle multiple numeric parameters', () => {
      const mockResponse = JSON.stringify({ success: true });
      mockRawApi.SetInfiniteRoyalePoints.mockReturnValue(mockResponse);

      const result = api.save.progress.setInfiniteRoyalePoints(1, 999999, 888888);

      expect(mockRawApi.SetInfiniteRoyalePoints).toHaveBeenCalledWith(1, 999999, 888888);
      expect(result).toHaveProperty('success', true);
    });
  });

  describe('Save Configuration Methods', () => {
    it('save.setTextSpeed should call SetTextSpeed', () => {
      const mockResponse = JSON.stringify({ success: true });
      mockRawApi.SetTextSpeed.mockReturnValue(mockResponse);

      const result = api.save.setTextSpeed(1, 3);

      expect(mockRawApi.SetTextSpeed).toHaveBeenCalledWith(1, 3);
      expect(result).toHaveProperty('success', true);
    });

    it('save.getTextSpeed should call GetTextSpeed', () => {
      const mockResponse = JSON.stringify({ speed: 2 });
      mockRawApi.GetTextSpeed.mockReturnValue(mockResponse);

      const result = api.save.getTextSpeed(1);

      expect(mockRawApi.GetTextSpeed).toHaveBeenCalledWith(1);
      expect(result).toHaveProperty('speed', 2);
    });
  });

  describe('Save Features Methods', () => {
    it('save.features.unlockFashionCategory should handle string parameter', () => {
      const mockResponse = JSON.stringify({ itemsUnlocked: 15 });
      mockRawApi.UnlockFashionCategory.mockReturnValue(mockResponse);

      const result = api.save.features.unlockFashionCategory(1, 'tops');

      expect(mockRawApi.UnlockFashionCategory).toHaveBeenCalledWith(1, 'tops');
      expect(result).toHaveProperty('itemsUnlocked', 15);
    });

    it('save.features.unlockAllFashion should call UnlockAllFashion', () => {
      const mockResponse = JSON.stringify({ itemsUnlocked: 150 });
      mockRawApi.UnlockAllFashion.mockReturnValue(mockResponse);

      const result = api.save.features.unlockAllFashion(1);

      expect(mockRawApi.UnlockAllFashion).toHaveBeenCalledWith(1);
      expect(result).toHaveProperty('itemsUnlocked', 150);
    });

    it('save.features.unlockAllHairMakeup should call UnlockAllHairMakeup', () => {
      const mockResponse = JSON.stringify({ itemsUnlocked: 50 });
      mockRawApi.UnlockAllHairMakeup.mockReturnValue(mockResponse);

      const result = api.save.features.unlockAllHairMakeup(1);

      expect(mockRawApi.UnlockAllHairMakeup).toHaveBeenCalledWith(1);
      expect(result).toHaveProperty('itemsUnlocked', 50);
    });
  });

  describe('Error Handling', () => {
    it('should handle invalid JSON gracefully', () => {
      mockRawApi.LoadSave.mockReturnValue('invalid json{');

      const result = api.save.load('data');

      expect(result).toHaveProperty('error');
      expect(result.code).toBe('PARSE_ERROR');
    });

    it('should preserve error responses from C# API', () => {
      const mockResponse = JSON.stringify({ error: 'Test error', code: 'TEST_CODE' });
      mockRawApi.LoadSave.mockReturnValue(mockResponse);

      const result = api.save.load('data');

      expect(result).toEqual({ error: 'Test error', code: 'TEST_CODE' });
    });

    it('should handle parse errors in nested operations', () => {
      mockRawApi.GetPokemon.mockReturnValue('invalid{json');

      const result = api.save.pokemon.get(1, 0, 0);

      expect(result).toHaveProperty('error');
      expect(result.code).toBe('PARSE_ERROR');
    });
  });
});
