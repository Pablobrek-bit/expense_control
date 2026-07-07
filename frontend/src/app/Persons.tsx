import React, { useEffect, useState } from 'react';
import { personService } from '../services/persons';
import type { Person, PaginatedResponse } from '../types';
import { Loader2, Plus, Trash2, Pencil } from 'lucide-react';

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
  const [isSubmitting, setIsSubmitting] = useState(false);

  const openModalForCreate = () => {
    setEditingId(null);
    setNewName('');
    setNewAge('');
    setIsModalOpen(true);
  };

  const openModalForEdit = (person: Person) => {
    setEditingId(person.id);
    setNewName(person.name);
    setNewAge(person.age);
    setIsModalOpen(true);
  };

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
    } catch (err) {
      console.error(err);
      alert(editingId ? 'Erro ao atualizar pessoa.' : 'Erro ao criar pessoa.');
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
                  <td className="px-6 py-4 text-right">
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
    </div>
  );
}
