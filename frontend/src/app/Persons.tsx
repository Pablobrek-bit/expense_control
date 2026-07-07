import React, { useEffect, useState } from 'react';
import { personService } from '../services/persons';
import type { Person, PaginatedResponse } from '../types';
import { Loader2, Plus, Trash2, Pencil, Eye } from 'lucide-react';

export default function Persons() {
  const [data, setData] = useState<PaginatedResponse<Person> | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [page, setPage] = useState(1);
  const pageSize = 10;

  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [newName, setNewName] = useState('');
  const [newAge, setNewAge] = useState<number | ''>('');
  const [originalAge, setOriginalAge] = useState<number | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [uxWarning, setUxWarning] = useState('');

  const [isDetailsModalOpen, setIsDetailsModalOpen] = useState(false);
  const [selectedPersonDetails, setSelectedPersonDetails] = useState<Person | null>(null);
  const [loadingDetails, setLoadingDetails] = useState(false);

  const openModalForCreate = () => {
    setEditingId(null);
    setNewName('');
    setNewAge('');
    setOriginalAge(null);
    setIsModalOpen(true);
  };

  const openModalForEdit = (person: Person) => {
    setEditingId(person.id);
    setNewName(person.name);
    setNewAge(person.age);
    setOriginalAge(person.age);
    setIsModalOpen(true);
  };

  useEffect(() => {
    setUxWarning('');
    if (editingId && originalAge !== null && originalAge >= 18 && newAge !== '' && newAge < 18) {
      setUxWarning('Atenção: Se esta pessoa possuir transações de Receita (Income) cadastradas, o servidor recusará a alteração para menor de idade.');
    }
  }, [editingId, originalAge, newAge]);

  const fetchPersons = async (currentPage: number) => {
    try {
      setLoading(true);
      const res = await personService.getAll(currentPage, pageSize);
      setData(res);
    } catch (err) {
      console.error(err);
      setError('Falha ao carregar pessoas.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchPersons(page);
  }, [page]);

  const handleViewDetails = async (id: string) => {
    setIsDetailsModalOpen(true);
    setLoadingDetails(true);
    setSelectedPersonDetails(null);
    try {
      const details = await personService.getById(id);
      setSelectedPersonDetails(details);
    } catch (err) {
      console.error(err);
      alert('Erro ao carregar detalhes da pessoa.');
      setIsDetailsModalOpen(false);
    } finally {
      setLoadingDetails(false);
    }
  };

  const handleDelete = async (id: string) => {
    if (!window.confirm('Tem certeza que deseja excluir esta pessoa?')) return;
    try {
      await personService.delete(id);
      fetchPersons(page);
    } catch (err) {
      console.error(err);
      alert('Erro ao excluir pessoa. Verifique se ela possui transações vinculadas.');
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!newName || !newAge) return;

    setIsSubmitting(true);
    try {
      if (editingId) {
        await personService.update(editingId, { name: newName, age: Number(newAge) });
      } else {
        await personService.create({ name: newName, age: Number(newAge) });
      }
      setIsModalOpen(false);
      setNewName('');
      setNewAge('');
      fetchPersons(page);
    } catch (err: any) {
      console.error(err);
      const errorMessage = err.response?.data?.detail || err.response?.data?.title || (editingId ? 'Erro ao atualizar pessoa.' : 'Erro ao criar pessoa.');
      alert(`Falha: ${errorMessage}`);
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <h2 className="text-2xl font-bold text-gray-800">Pessoas</h2>
        <button
          onClick={openModalForCreate}
          className="flex items-center space-x-2 bg-emerald-600 hover:bg-emerald-700 text-white px-4 py-2 rounded-lg transition-colors"
        >
          <Plus size={20} />
          <span>Nova Pessoa</span>
        </button>
      </div>

      {error && (
        <div className="bg-red-50 text-red-600 p-4 rounded-lg border border-red-200">
          {error}
        </div>
      )}

      <div className="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full text-left">
            <thead className="text-xs text-gray-500 uppercase bg-gray-50 border-b border-gray-100">
              <tr>
                <th className="px-6 py-4 font-semibold">Nome</th>
                <th className="px-6 py-4 font-semibold">Idade</th>
                <th className="px-6 py-4 font-semibold text-right">Ações</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100">
              {loading ? (
                <tr>
                  <td colSpan={3} className="px-6 py-8 text-center">
                    <Loader2 className="animate-spin text-emerald-500 mx-auto" size={32} />
                  </td>
                </tr>
              ) : data?.items.map((person) => (
                <tr key={person.id} className="hover:bg-gray-50 transition-colors">
                  <td className="px-6 py-4 font-medium text-gray-900">{person.name}</td>
                  <td className="px-6 py-4 text-gray-600">{person.age} anos</td>
                  <td className="px-6 py-4 text-right whitespace-nowrap">
                    <button
                      onClick={() => handleViewDetails(person.id)}
                      className="text-emerald-500 hover:text-emerald-700 p-2 rounded-full hover:bg-emerald-50 transition-colors inline-flex mr-2"
                      title="Ver detalhes"
                    >
                      <Eye size={18} />
                    </button>
                    <button
                      onClick={() => openModalForEdit(person)}
                      className="text-blue-500 hover:text-blue-700 p-2 rounded-full hover:bg-blue-50 transition-colors inline-flex mr-2"
                      title="Editar pessoa"
                    >
                      <Pencil size={18} />
                    </button>
                    <button
                      onClick={() => handleDelete(person.id)}
                      className="text-red-500 hover:text-red-700 p-2 rounded-full hover:bg-red-50 transition-colors inline-flex"
                      title="Excluir pessoa"
                    >
                      <Trash2 size={18} />
                    </button>
                  </td>
                </tr>
              ))}
              
              {!loading && data?.items.length === 0 && (
                <tr>
                  <td colSpan={3} className="px-6 py-8 text-center text-gray-500">
                    Nenhuma pessoa cadastrada. Adicione sua primeira pessoa!
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
        
        {/* Controles de Paginação */}
        {data && data.totalCount > pageSize && (
          <div className="px-6 py-4 border-t border-gray-100 bg-gray-50 flex items-center justify-between">
            <span className="text-sm text-gray-500">
              Mostrando {((data.page - 1) * data.pageSize) + 1} a {Math.min(data.page * data.pageSize, data.totalCount)} de {data.totalCount} registros
            </span>
            <div className="flex space-x-2">
              <button
                onClick={() => setPage(p => Math.max(1, p - 1))}
                disabled={data.page === 1}
                className="px-3 py-1 border border-gray-300 rounded hover:bg-gray-200 disabled:opacity-50 text-sm font-medium transition-colors"
              >
                Anterior
              </button>
              <button
                onClick={() => setPage(p => p + 1)}
                disabled={data.page * data.pageSize >= data.totalCount}
                className="px-3 py-1 border border-gray-300 rounded hover:bg-gray-200 disabled:opacity-50 text-sm font-medium transition-colors"
              >
                Próxima
              </button>
            </div>
          </div>
        )}
      </div>

      {isModalOpen && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center p-4 z-50">
          <div className="bg-white rounded-xl shadow-lg w-full max-w-md overflow-hidden">
            <div className="px-6 py-4 border-b border-gray-100 flex justify-between items-center">
              <h3 className="text-lg font-bold text-gray-800">{editingId ? 'Editar Pessoa' : 'Nova Pessoa'}</h3>
              <button onClick={() => setIsModalOpen(false)} className="text-gray-400 hover:text-gray-600 text-2xl leading-none">&times;</button>
            </div>
            
            <form onSubmit={handleSubmit} className="p-6 space-y-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Nome</label>
                <input
                  type="text"
                  required
                  value={newName}
                  onChange={(e) => setNewName(e.target.value)}
                  className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-emerald-500 focus:border-emerald-500 outline-none transition-all"
                  placeholder="Ex: João Silva"
                />
              </div>
              
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Idade</label>
                <input
                  type="number"
                  required
                  min="1"
                  max="150"
                  value={newAge}
                  onChange={(e) => setNewAge(e.target.value ? Number(e.target.value) : '')}
                  className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-emerald-500 focus:border-emerald-500 outline-none transition-all"
                  placeholder="Ex: 30"
                />
              </div>

              {uxWarning && (
                <div className="bg-orange-50 text-orange-700 p-3 rounded-lg text-sm font-medium border border-orange-200">
                  {uxWarning}
                </div>
              )}

              <div className="pt-4 flex space-x-3">
                <button
                  type="button"
                  onClick={() => setIsModalOpen(false)}
                  className="flex-1 bg-gray-100 hover:bg-gray-200 text-gray-700 font-medium py-2 px-4 rounded-lg transition-colors"
                >
                  Cancelar
                </button>
                <button
                  type="submit"
                  disabled={isSubmitting}
                  className="flex-1 bg-emerald-600 hover:bg-emerald-700 text-white font-medium py-2 px-4 rounded-lg transition-colors flex justify-center items-center"
                >
                  {isSubmitting ? <Loader2 className="animate-spin" size={20} /> : 'Salvar'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* Modal de Detalhes da Pessoa */}
      {isDetailsModalOpen && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center p-4 z-50">
          <div className="bg-white rounded-xl shadow-lg w-full max-w-2xl overflow-hidden flex flex-col max-h-[90vh]">
            <div className="px-6 py-4 border-b border-gray-100 flex justify-between items-center bg-gray-50">
              <h3 className="text-xl font-bold text-gray-800 flex items-center gap-2">
                <Eye size={24} className="text-emerald-600" /> Detalhes da Pessoa
              </h3>
              <button onClick={() => setIsDetailsModalOpen(false)} className="text-gray-400 hover:text-gray-600 text-2xl leading-none">&times;</button>
            </div>
            
            <div className="p-6 overflow-y-auto flex-1">
              {loadingDetails ? (
                <div className="flex flex-col items-center justify-center py-12">
                  <Loader2 className="animate-spin text-emerald-500 mb-4" size={40} />
                  <p className="text-gray-500">Carregando histórico...</p>
                </div>
              ) : selectedPersonDetails ? (
                <div className="space-y-6">
                  <div className="flex justify-between items-center bg-emerald-50 p-4 rounded-lg border border-emerald-100">
                    <div>
                      <p className="text-sm text-emerald-800 font-medium uppercase tracking-wide">Nome</p>
                      <p className="text-2xl font-bold text-gray-900">{selectedPersonDetails.name}</p>
                    </div>
                    <div className="text-right">
                      <p className="text-sm text-emerald-800 font-medium uppercase tracking-wide">Idade</p>
                      <p className="text-2xl font-bold text-gray-900">{selectedPersonDetails.age} anos</p>
                    </div>
                  </div>

                  <div>
                    <h4 className="text-lg font-bold text-gray-800 mb-3 border-b pb-2">Histórico de Transações</h4>
                    
                    {!selectedPersonDetails.transactions || selectedPersonDetails.transactions.length === 0 ? (
                      <div className="text-center py-8 bg-gray-50 rounded-lg border border-gray-100">
                        <p className="text-gray-500">Nenhuma transação encontrada para esta pessoa.</p>
                      </div>
                    ) : (
                      <div className="space-y-3">
                        {selectedPersonDetails.transactions.map((tx) => (
                          <div key={tx.id} className="flex items-center justify-between p-4 bg-white border border-gray-100 shadow-sm rounded-lg hover:border-emerald-200 transition-colors">
                            <div className="flex items-center gap-4">
                              <div className={`p-2 rounded-full ${tx.type === 'Income' ? 'bg-emerald-100 text-emerald-600' : 'bg-red-100 text-red-600'}`}>
                                {tx.type === 'Income' ? (
                                  <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><circle cx="12" cy="12" r="10"/><path d="m16 12-4-4-4 4"/><path d="M12 8v8"/></svg>
                                ) : (
                                  <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><circle cx="12" cy="12" r="10"/><path d="m8 12 4 4 4-4"/><path d="M12 8v8"/></svg>
                                )}
                              </div>
                              <div>
                                <p className="font-semibold text-gray-900">{tx.description}</p>
                                <p className="text-xs text-gray-500 font-medium uppercase">{tx.type === 'Income' ? 'Receita' : 'Despesa'}</p>
                              </div>
                            </div>
                            <span className={`font-bold text-lg ${tx.type === 'Income' ? 'text-emerald-600' : 'text-red-600'}`}>
                              {tx.type === 'Income' ? '+' : '-'} R$ {tx.value.toFixed(2)}
                            </span>
                          </div>
                        ))}
                      </div>
                    )}
                  </div>
                </div>
              ) : (
                <div className="text-center py-8 text-red-500">
                  <p>Não foi possível carregar os dados.</p>
                </div>
              )}
            </div>
            
            <div className="px-6 py-4 border-t border-gray-100 bg-gray-50">
              <button
                onClick={() => setIsDetailsModalOpen(false)}
                className="w-full bg-white border border-gray-300 hover:bg-gray-50 text-gray-700 font-medium py-2 px-4 rounded-lg transition-colors shadow-sm"
              >
                Fechar
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
